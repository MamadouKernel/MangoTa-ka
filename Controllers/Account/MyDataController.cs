using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using System.Security.Claims;

namespace MangoTaikaDistrict.Controllers.Account;

[Authorize]
public class MyDataController : Controller
{
    private readonly IUtilisateurRepository _users;
    private readonly IScoutRepository _scouts;
    private readonly IDemandeRgpdRepository _demandesRgpd;
    private readonly ICompetenceRepository _competences;

    public MyDataController(
        IUtilisateurRepository users, 
        IScoutRepository scouts, 
        IDemandeRgpdRepository demandesRgpd,
        ICompetenceRepository competences)
    {
        _users = users;
        _scouts = scouts;
        _demandesRgpd = demandesRgpd;
        _competences = competences;
    }

    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _users.GetByIdAsync(userId);
        
        // Si l'utilisateur est un scout, récupérer ses données scout
        var scouts = await _scouts.GetByUtilisateurIdAsync(userId);
        var scout = scouts.FirstOrDefault();
        
        // Récupérer les compétences du scout
        var scoutCompetences = scout != null 
            ? await _competences.GetScoutCompetencesAsync(scout.Id) 
            : new List<ScoutCompetence>();

        // Récupérer les demandes RGPD de l'utilisateur
        var demandes = await _demandesRgpd.GetByUtilisateurAsync(userId);
        
        ViewBag.User = user;
        ViewBag.Scout = scout;
        ViewBag.Scouts = scouts;
        ViewBag.Competences = scoutCompetences;
        ViewBag.Demandes = demandes;
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RequestDeletion(string? reason)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var demande = new DemandeDroitRgpd
        {
            UtilisateurId = userId,
            Type = TypeDemandeRgpd.SUPPRESSION,
            Raison = reason,
            Statut = StatutDemandeRgpd.EN_ATTENTE
        };

        await _demandesRgpd.AddAsync(demande);
        await _demandesRgpd.SaveAsync();

        TempData["Success"] = "Votre demande de suppression a été enregistrée. Un administrateur la traitera sous peu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RequestRectification(string field, string newValue, string? description)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var demande = new DemandeDroitRgpd
        {
            UtilisateurId = userId,
            Type = TypeDemandeRgpd.RECTIFICATION,
            Description = $"Champ: {field}, Nouvelle valeur: {newValue}",
            Raison = description,
            Statut = StatutDemandeRgpd.EN_ATTENTE
        };

        await _demandesRgpd.AddAsync(demande);
        await _demandesRgpd.SaveAsync();

        TempData["Success"] = "Votre demande de rectification a été enregistrée.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RequestAccess()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var demande = new DemandeDroitRgpd
        {
            UtilisateurId = userId,
            Type = TypeDemandeRgpd.ACCES,
            Statut = StatutDemandeRgpd.EN_ATTENTE
        };

        await _demandesRgpd.AddAsync(demande);
        await _demandesRgpd.SaveAsync();

        TempData["Success"] = "Votre demande d'accès a été enregistrée. Vous recevrez un export de vos données sous peu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RequestOpposition(string? reason)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var demande = new DemandeDroitRgpd
        {
            UtilisateurId = userId,
            Type = TypeDemandeRgpd.OPPOSITION,
            Raison = reason,
            Statut = StatutDemandeRgpd.EN_ATTENTE
        };

        await _demandesRgpd.AddAsync(demande);
        await _demandesRgpd.SaveAsync();

        TempData["Success"] = "Votre demande d'opposition a été enregistrée.";
        return RedirectToAction(nameof(Index));
    }
}
