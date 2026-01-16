using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Infrastructure.Repositories;
using System.Security.Claims;

namespace MangoTaikaDistrict.Controllers.Lms;

[Authorize(Roles = "SCOUT")]
public class MesFormationsController : Controller
{
    private readonly IInscriptionFormationRepository _inscriptions;
    private readonly IScoutRepository _scouts;

    public MesFormationsController(
        IInscriptionFormationRepository inscriptions,
        IScoutRepository scouts)
    {
        _inscriptions = inscriptions;
        _scouts = scouts;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var userIdGuid))
            return Unauthorized();

        var scouts = await _scouts.GetByUtilisateurIdAsync(userIdGuid);
        var scout = scouts.FirstOrDefault();
        if (scout == null)
        {
            TempData["Error"] = "Profil scout introuvable.";
            return RedirectToAction("Index", "Home");
        }

        var inscriptions = await _inscriptions.GetByScoutIdAsync(scout.Id);
        return View(inscriptions);
    }

    [HttpGet]
    public async Task<IActionResult> Suivre(Guid inscriptionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var userIdGuid))
            return Unauthorized();

        var scouts = await _scouts.GetByUtilisateurIdAsync(userIdGuid);
        var scout = scouts.FirstOrDefault();
        if (scout == null) return Unauthorized();

        var inscription = await _inscriptions.GetByIdWithDetailsAsync(inscriptionId);
        if (inscription == null || inscription.ScoutId != scout.Id)
            return NotFound();

        return View(inscription);
    }

    [HttpPost]
    public async Task<IActionResult> MarquerModuleComplete(Guid inscriptionId, Guid moduleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var userIdGuid))
            return Unauthorized();

        var scouts = await _scouts.GetByUtilisateurIdAsync(userIdGuid);
        var scout = scouts.FirstOrDefault();
        if (scout == null) return Unauthorized();

        var inscription = await _inscriptions.GetByIdWithDetailsAsync(inscriptionId);
        if (inscription == null || inscription.ScoutId != scout.Id)
            return NotFound();

        var progression = inscription.Progressions.FirstOrDefault(p => p.ModuleFormationId == moduleId);
        if (progression == null)
        {
            progression = new Domain.Entities.ProgressionModule
            {
                InscriptionFormationId = inscriptionId,
                ModuleFormationId = moduleId,
                DateDebut = DateTime.UtcNow
            };
            inscription.Progressions.Add(progression);
        }

        progression.EstComplete = true;
        progression.DateCompletion = DateTime.UtcNow;

        // Vérifier si tous les modules obligatoires sont complétés
        var modulesObligatoires = inscription.Formation.Modules.Where(m => m.EstObligatoire).ToList();
        var modulesCompletes = inscription.Progressions.Where(p => p.EstComplete).Select(p => p.ModuleFormationId).ToList();
        var tousCompletes = modulesObligatoires.All(m => modulesCompletes.Contains(m.Id));

        if (tousCompletes && inscription.Statut != Domain.Enums.StatutInscription.COMPLETE)
        {
            inscription.Statut = Domain.Enums.StatutInscription.COMPLETE;
            inscription.DateCompletion = DateTime.UtcNow;
        }
        else if (inscription.Statut == Domain.Enums.StatutInscription.INSCRIT)
        {
            inscription.Statut = Domain.Enums.StatutInscription.EN_COURS;
            inscription.DateDebut = DateTime.UtcNow;
        }

        await _inscriptions.UpdateAsync(inscription);
        await _inscriptions.SaveAsync();

        TempData["Success"] = "Module marqué comme complété !";
        return RedirectToAction(nameof(Suivre), new { inscriptionId });
    }
}
