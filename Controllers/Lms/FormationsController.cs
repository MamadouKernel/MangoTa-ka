using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Infrastructure.Repositories;
using System.Security.Claims;

namespace MangoTaikaDistrict.Controllers.Lms;

[Authorize]
public class FormationsController : Controller
{
    private readonly IFormationRepository _formations;
    private readonly IInscriptionFormationRepository _inscriptions;
    private readonly IScoutRepository _scouts;

    public FormationsController(
        IFormationRepository formations,
        IInscriptionFormationRepository inscriptions,
        IScoutRepository scouts)
    {
        _formations = formations;
        _inscriptions = inscriptions;
        _scouts = scouts;
    }

    public async Task<IActionResult> Index()
    {
        var formations = await _formations.GetAllAsync();
        return View(formations);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var formation = await _formations.GetByIdWithModulesAsync(id);
        if (formation == null) return NotFound();

        // Vérifier si l'utilisateur est inscrit
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userId, out var userIdGuid))
        {
            var scouts = await _scouts.GetByUtilisateurIdAsync(userIdGuid);
            var scout = scouts.FirstOrDefault();
            if (scout != null)
            {
                var inscription = await _inscriptions.GetByScoutAndFormationAsync(scout.Id, id);
                ViewBag.Inscription = inscription;
            }
        }

        return View(formation);
    }

    [HttpPost]
    [Authorize(Roles = "SCOUT")]
    public async Task<IActionResult> SInscrire(Guid formationId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var userIdGuid))
            return Unauthorized();

        var scouts = await _scouts.GetByUtilisateurIdAsync(userIdGuid);
        var scout = scouts.FirstOrDefault();
        if (scout == null)
        {
            TempData["Error"] = "Profil scout introuvable.";
            return RedirectToAction(nameof(Index));
        }

        // Vérifier si déjà inscrit
        var existing = await _inscriptions.GetByScoutAndFormationAsync(scout.Id, formationId);
        if (existing != null)
        {
            TempData["Error"] = "Vous êtes déjà inscrit à cette formation.";
            return RedirectToAction(nameof(Details), new { id = formationId });
        }

        var formation = await _formations.GetByIdWithModulesAsync(formationId);
        if (formation == null) return NotFound();

        var inscription = new Domain.Entities.InscriptionFormation
        {
            FormationId = formationId,
            ScoutId = scout.Id,
            Statut = Domain.Enums.StatutInscription.INSCRIT,
            DateInscription = DateTime.UtcNow
        };

        await _inscriptions.AddAsync(inscription);
        await _inscriptions.SaveAsync();

        TempData["Success"] = "Inscription réussie !";
        return RedirectToAction(nameof(Details), new { id = formationId });
    }
}
