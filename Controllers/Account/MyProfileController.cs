using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Domain.Entities;
using System.Security.Claims;

namespace MangoTaikaDistrict.Controllers.Account;

[Authorize(Roles = "SCOUT")]
public class MyProfileController : Controller
{
    private readonly IScoutRepository _scoutRepo;
    private readonly IUtilisateurRepository _userRepo;

    public MyProfileController(IScoutRepository scoutRepo, IUtilisateurRepository userRepo)
    {
        _scoutRepo = scoutRepo;
        _userRepo = userRepo;
    }

    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var scouts = await _scoutRepo.GetByUtilisateurIdAsync(userId);
        var scout = scouts.FirstOrDefault();

        if (scout == null)
        {
            TempData["Error"] = "Aucun profil scout associé à votre compte.";
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        return View(scout);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Scout scout)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var scouts = await _scoutRepo.GetByUtilisateurIdAsync(userId);
        var existingScout = scouts.FirstOrDefault(s => s.Id == scout.Id);

        if (existingScout == null)
        {
            return Forbid();
        }

        // Mise à jour uniquement des champs autorisés pour un scout
        existingScout.Adresse = scout.Adresse;
        existingScout.GpsLat = scout.GpsLat;
        existingScout.GpsLng = scout.GpsLng;
        existingScout.UpdatedAt = DateTime.UtcNow;

        await _scoutRepo.UpdateAsync(existingScout);
        await _scoutRepo.SaveAsync();

        TempData["Success"] = "Votre profil a été mis à jour avec succès.";
        return RedirectToAction(nameof(Index));
    }
}
