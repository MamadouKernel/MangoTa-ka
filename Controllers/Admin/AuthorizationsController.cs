using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Application.Services;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Domain.Entities;
using System.Security.Claims;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize]
[Area("Admin")]
public class AuthorizationsController : Controller
{
    private readonly IWorkflowRepository _repo;
    private readonly WorkflowService _service;
    private readonly IGroupeRepository _groupes;

    public AuthorizationsController(IWorkflowRepository repo, WorkflowService service, IGroupeRepository groupes)
    {
        _repo = repo; _service = service; _groupes = groupes;
    }

    public async Task<IActionResult> Activites()
        => View(await _repo.GetAllActivitesAsync());

    [Authorize(Roles = "ADMIN,GESTIONNAIRE")]
    [HttpGet]
    public async Task<IActionResult> CreateActivite()
    {
        ViewBag.Groupes = await _groupes.GetAllAsync();
        return View(new Activite { DateDebut = DateTime.UtcNow, DateFin = DateTime.UtcNow.AddHours(2) });
    }

    [Authorize(Roles = "ADMIN,GESTIONNAIRE")]
    [HttpPost]
    public async Task<IActionResult> CreateActivite(Activite a)
    {
        if (string.IsNullOrWhiteSpace(a.Titre) || a.GroupeId == Guid.Empty)
        {
            ModelState.AddModelError("", "Titre et Groupe obligatoires.");
            ViewBag.Groupes = await _groupes.GetAllAsync();
            return View(a);
        }

        a.CreatedById = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _repo.AddActiviteAsync(a);
        await _repo.SaveAsync();
        return RedirectToAction(nameof(Activites));
    }

    [Authorize(Roles = "ADMIN,GESTIONNAIRE")]
    [HttpPost]
    public async Task<IActionResult> Soumettre(Guid activiteId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (ok, error) = await _service.SoumettreAutorisationAsync(activiteId, userId);
        if (!ok) TempData["err"] = error;
        return RedirectToAction(nameof(Activites));
    }

    public async Task<IActionResult> Pending()
    {
        // Les rôles du user déterminent ce qu’il peut voir
        var roles = User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();
        var pending = new List<DemandeAutorisation>();
        foreach (var r in roles)
        {
            pending.AddRange(await _repo.GetDemandesEnAttenteAsync(r));
        }
        // évite doublons
        pending = pending.GroupBy(x => x.Id).Select(g => g.First()).ToList();
        return View(pending);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var d = await _repo.GetDemandeAsync(id);
        if (d is null) return NotFound();
        return View(d);
    }

    [HttpPost]
    public async Task<IActionResult> Decide(Guid id, string decision, string? commentaire)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var approve = decision == "approve";
        var (ok, error) = await _service.DeciderAsync(id, userId, approve, commentaire);
        if (!ok) TempData["err"] = error;
        return RedirectToAction(nameof(Details), new { id });
    }
}
