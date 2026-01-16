using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Infrastructure.Storage;
using System.Security.Claims;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize(Roles = "ADMIN,GESTIONNAIRE")]
[Area("Admin")]
public class MotCommissaireController : Controller
{
    private readonly IMotCommissaireRepository _repo;
    private readonly IFileStorageService _storage;

    public MotCommissaireController(IMotCommissaireRepository repo, IFileStorageService storage)
    {
        _repo = repo;
        _storage = storage;
    }

    public async Task<IActionResult> Index()
        => View(await _repo.GetAllAsync());

    [HttpGet]
    public IActionResult Create() => View(new MotCommissaire
    {
        Annee = DateTime.UtcNow.Year,
        DateDebut = DateTime.UtcNow,
        DateFin = DateTime.UtcNow.AddYears(1)
    });

    [HttpPost]
    public async Task<IActionResult> Create(MotCommissaire mot, IFormFile? photo)
    {
        if (!ModelState.IsValid) return View(mot);

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        mot.CreatedById = userId;
        mot.CreatedAt = DateTime.UtcNow;
        mot.UpdatedAt = DateTime.UtcNow;

        // Désactiver les autres mots actifs pour cette année
        var existing = await _repo.GetAllAsync();
        foreach (var existingMot in existing.Where(x => x.Annee == mot.Annee && x.IsActive))
        {
            existingMot.IsActive = false;
            await _repo.UpdateAsync(existingMot);
        }

        if (photo != null && photo.Length > 0)
        {
            var (filePath, fileName, contentType, size) = await _storage.SaveAsync(photo, "commissaire");
            mot.PhotoUrl = filePath;
        }

        await _repo.AddAsync(mot);
        await _repo.SaveAsync();

        TempData["Success"] = "Mot du commissaire créé avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var mot = await _repo.GetByIdAsync(id);
        if (mot == null) return NotFound();
        return View(mot);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MotCommissaire mot, IFormFile? photo)
    {
        if (!ModelState.IsValid) return View(mot);

        var db = await _repo.GetByIdAsync(mot.Id);
        if (db == null) return NotFound();

        db.Titre = mot.Titre;
        db.Contenu = mot.Contenu;
        db.Annee = mot.Annee;
        db.DateDebut = mot.DateDebut;
        db.DateFin = mot.DateFin;
        db.IsActive = mot.IsActive;
        db.UpdatedAt = DateTime.UtcNow;

        if (photo != null && photo.Length > 0)
        {
            var (filePath, fileName, contentType, size) = await _storage.SaveAsync(photo, "commissaire");
            db.PhotoUrl = filePath;
        }

        await _repo.UpdateAsync(db);
        await _repo.SaveAsync();

        TempData["Success"] = "Mot du commissaire modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }
}
