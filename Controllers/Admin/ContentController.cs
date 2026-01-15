using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Application.Services;
using System.Security.Claims;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize(Roles = "ADMIN,GESTIONNAIRE")]
[Area("Admin")]
public class ContentController : Controller
{
    private readonly IContentRepository _content;
    private readonly DocumentService _docs;

    public ContentController(IContentRepository content, DocumentService docs)
    {
        _content = content;
        _docs = docs;
    }

    // --- NEWS ---
    public async Task<IActionResult> News() => View(await _content.GetAllNewsAsync());

    [HttpGet]
    public IActionResult CreateNews() => View(new Actualite());

    [HttpPost]
    public async Task<IActionResult> CreateNews(Actualite a)
    {
        if (string.IsNullOrWhiteSpace(a.Titre) || string.IsNullOrWhiteSpace(a.Contenu))
        {
            ModelState.AddModelError("", "Titre et contenu obligatoires.");
            return View(a);
        }

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        a.CreatedById = userId;
        a.IsPublished = true;
        a.PublishedAt = DateTime.UtcNow;

        await _content.AddNewsAsync(a);
        await _content.SaveAsync();
        return RedirectToAction(nameof(News));
    }

    // --- GUESTBOOK moderation ---
    public async Task<IActionResult> Guestbook() => View(await _content.GetGuestbookAllAsync());

    [HttpPost]
    public async Task<IActionResult> Moderate(Guid id, string action)
    {
        var msg = await _content.GetGuestbookAsync(id);
        if (msg is null) return NotFound();

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        msg.Statut = action == "approve" ? StatutModeration.VALIDE : StatutModeration.REJETE;
        msg.ModeratedById = userId;
        msg.ModeratedAt = DateTime.UtcNow;

        await _content.SaveAsync();
        return RedirectToAction(nameof(Guestbook));
    }

    // --- Albums + Upload media ---
    public async Task<IActionResult> Albums() => View(await _content.GetAllAlbumsAsync());

    [HttpGet]
    public IActionResult CreateAlbum() => View(new Album());

    [HttpPost]
    public async Task<IActionResult> CreateAlbum(Album a)
    {
        if (string.IsNullOrWhiteSpace(a.Titre))
        {
            ModelState.AddModelError("", "Titre obligatoire.");
            return View(a);
        }
        await _content.AddAlbumAsync(a);
        await _content.SaveAsync();
        return RedirectToAction(nameof(Albums));
    }

    [HttpGet]
    public async Task<IActionResult> UploadMedia(Guid albumId)
    {
        var album = await _content.GetAlbumAsync(albumId);
        if (album is null) return NotFound();
        ViewBag.Album = album;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadMedia(Guid albumId, IFormFile file, string? caption)
    {
        var album = await _content.GetAlbumAsync(albumId);
        if (album is null) return NotFound();
        if (file == null) return BadRequest("Fichier obligatoire.");

        // On réutilise DocumentService pour sauvegarde fichier (ownerType=MEDIA)
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        // sauvegarde physique
        var storage = HttpContext.RequestServices.GetRequiredService<MangoTaikaDistrict.Infrastructure.Storage.IFileStorageService>();
        var (path, _, _, _) = await storage.SaveAsync(file, "media");

        await _content.AddMediaAsync(new Media
        {
            AlbumId = albumId,
            FilePath = path,
            Caption = caption,
            MediaType = file.ContentType.StartsWith("video") ? "Video" : "Photo"
        });

        await _content.SaveAsync();
        return RedirectToAction(nameof(Albums));
    }
}
