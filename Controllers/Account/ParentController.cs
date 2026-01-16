using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Domain.Entities;
using System.Security.Claims;

namespace MangoTaikaDistrict.Controllers.Account;

[Authorize(Roles = "PARENT")]
public class ParentController : Controller
{
    private readonly IParentRepository _parentRepo;
    private readonly IScoutRepository _scoutRepo;
    private readonly ICompetenceRepository _competenceRepo;

    public ParentController(
        IParentRepository parentRepo,
        IScoutRepository scoutRepo,
        ICompetenceRepository competenceRepo)
    {
        _parentRepo = parentRepo;
        _scoutRepo = scoutRepo;
        _competenceRepo = competenceRepo;
    }

    public async Task<IActionResult> MesEnfants()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var enfants = await _parentRepo.GetEnfantsAsync(userId);

        // Récupérer les compétences pour chaque enfant
        var enfantsAvecCompetences = new List<object>();
        foreach (var enfant in enfants)
        {
            var competences = await _competenceRepo.GetScoutCompetencesAsync(enfant.Id);
            enfantsAvecCompetences.Add(new
            {
                Scout = enfant,
                Competences = competences
            });
        }

        ViewBag.Enfants = enfants;
        ViewBag.EnfantsAvecCompetences = enfantsAvecCompetences;

        return View();
    }

    public async Task<IActionResult> DetailsEnfant(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var enfants = await _parentRepo.GetEnfantsAsync(userId);

        // Vérifier que l'enfant appartient bien au parent
        if (!enfants.Any(e => e.Id == id))
        {
            return Forbid();
        }

        var scout = await _scoutRepo.GetByIdAsync(id);
        if (scout == null) return NotFound();

        var competences = await _competenceRepo.GetScoutCompetencesAsync(scout.Id);
        var cotisations = scout.Cotisations.OrderByDescending(c => c.Periode).ToList();
        var nominations = scout.Nominations.OrderByDescending(n => n.DateDebut).ToList();

        ViewBag.Scout = scout;
        ViewBag.Competences = competences;
        ViewBag.Cotisations = cotisations;
        ViewBag.Nominations = nominations;

        return View();
    }
}
