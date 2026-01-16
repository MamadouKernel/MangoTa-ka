using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Domain.Entities;
using System.Security.Claims;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize(Roles = "ADMIN,GESTIONNAIRE")]
[Area("Admin")]
public class AscciController : Controller
{
    private readonly IAscciStatusRepository _ascciRepo;
    private readonly IScoutRepository _scoutRepo;

    public AscciController(IAscciStatusRepository ascciRepo, IScoutRepository scoutRepo)
    {
        _ascciRepo = ascciRepo;
        _scoutRepo = scoutRepo;
    }

    public async Task<IActionResult> Index()
    {
        var allStatuses = await _ascciRepo.GetAllAsync();
        var expired = await _ascciRepo.GetExpiredAsync();
        var expiringSoon = await _ascciRepo.GetExpiringSoonAsync();

        ViewBag.AllStatuses = allStatuses;
        ViewBag.Expired = expired;
        ViewBag.ExpiringSoon = expiringSoon;

        return View();
    }

    public async Task<IActionResult> CheckScout(Guid scoutId)
    {
        var scout = await _scoutRepo.GetByIdAsync(scoutId);
        if (scout == null) return NotFound();

        var existingStatus = await _ascciRepo.GetByScoutIdAsync(scoutId);
        ViewBag.Scout = scout;
        ViewBag.ExistingStatus = existingStatus;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(Guid scoutId, string numeroCarte, string statut, DateTime? dateExpiration, string? observations)
    {
        var scout = await _scoutRepo.GetByIdAsync(scoutId);
        if (scout == null) return NotFound();

        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var existingStatus = await _ascciRepo.GetByScoutIdAsync(scoutId);

        if (existingStatus == null)
        {
            existingStatus = new AscciStatus
            {
                ScoutId = scoutId,
                NumeroCarte = numeroCarte,
                Statut = statut,
                DateExpiration = dateExpiration,
                Observations = observations,
                VerifieParId = currentUserId,
                DateVerification = DateTime.UtcNow
            };
            await _ascciRepo.AddAsync(existingStatus);
        }
        else
        {
            existingStatus.NumeroCarte = numeroCarte;
            existingStatus.Statut = statut;
            existingStatus.DateExpiration = dateExpiration;
            existingStatus.Observations = observations;
            existingStatus.VerifieParId = currentUserId;
            existingStatus.DateVerification = DateTime.UtcNow;
            existingStatus.UpdatedAt = DateTime.UtcNow;
            await _ascciRepo.UpdateAsync(existingStatus);
        }

        await _ascciRepo.SaveAsync();

        TempData["Success"] = "Statut ASCCI mis à jour avec succès.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> CheckMultiple(string[] scoutIds)
    {
        // Pour une vérification en masse (futur : intégration API)
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var checkedCount = 0;

        foreach (var scoutIdStr in scoutIds)
        {
            if (Guid.TryParse(scoutIdStr, out var scoutId))
            {
                var existingStatus = await _ascciRepo.GetByScoutIdAsync(scoutId);
                if (existingStatus == null)
                {
                    var status = new AscciStatus
                    {
                        ScoutId = scoutId,
                        Statut = "Non vérifié",
                        VerifieParId = currentUserId,
                        DateVerification = DateTime.UtcNow
                    };
                    await _ascciRepo.AddAsync(status);
                    checkedCount++;
                }
            }
        }

        if (checkedCount > 0)
        {
            await _ascciRepo.SaveAsync();
            TempData["Success"] = $"{checkedCount} scout(s) ajouté(s) à la liste de vérification.";
        }

        return RedirectToAction(nameof(Index));
    }
}
