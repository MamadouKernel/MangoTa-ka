using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using System.Security.Claims;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize(Roles = "ADMIN,GESTIONNAIRE")]
[Area("Admin")]
public class RgpdController : Controller
{
    private readonly IDemandeRgpdRepository _demandesRepo;
    private readonly IUtilisateurRepository _usersRepo;
    private readonly IScoutRepository _scoutsRepo;

    public RgpdController(
        IDemandeRgpdRepository demandesRepo,
        IUtilisateurRepository usersRepo,
        IScoutRepository scoutsRepo)
    {
        _demandesRepo = demandesRepo;
        _usersRepo = usersRepo;
        _scoutsRepo = scoutsRepo;
    }

    public async Task<IActionResult> Index()
    {
        var demandes = await _demandesRepo.GetPendingAsync();
        return View(demandes);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var demande = await _demandesRepo.GetByIdAsync(id);
        if (demande == null) return NotFound();

        // Récupérer les données de l'utilisateur selon le type de demande
        if (demande.Type == TypeDemandeRgpd.ACCES)
        {
            var user = await _usersRepo.GetByIdAsync(demande.UtilisateurId);
            var scouts = await _scoutsRepo.GetByUtilisateurIdAsync(demande.UtilisateurId);
            ViewBag.User = user;
            ViewBag.Scouts = scouts;
        }

        return View(demande);
    }

    [HttpPost]
    public async Task<IActionResult> Traiter(Guid id, string reponse, bool approuver)
    {
        var demande = await _demandesRepo.GetByIdAsync(id);
        if (demande == null) return NotFound();

        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        demande.Statut = approuver ? StatutDemandeRgpd.TRAITEE : StatutDemandeRgpd.REJETEE;
        demande.TraiteParId = currentUserId;
        demande.TraiteLe = DateTime.UtcNow;
        demande.Reponse = reponse;
        demande.UpdatedAt = DateTime.UtcNow;

        // Si demande de suppression approuvée, désactiver le compte
        if (approuver && demande.Type == TypeDemandeRgpd.SUPPRESSION)
        {
            var user = await _usersRepo.GetByIdAsync(demande.UtilisateurId);
            if (user != null)
            {
                user.IsActive = false;
                await _usersRepo.UpdateAsync(user);
            }
        }

        await _demandesRepo.UpdateAsync(demande);
        await _demandesRepo.SaveAsync();

        TempData["Success"] = $"Demande {(approuver ? "approuvée" : "rejetée")} avec succès.";
        return RedirectToAction(nameof(Index));
    }
}
