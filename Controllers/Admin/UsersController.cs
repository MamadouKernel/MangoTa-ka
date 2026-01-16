using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Application.Interfaces;
using MangoTaikaDistrict.Infrastructure.Repositories;
using System.Security.Claims;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize(Roles = "ADMIN,GESTIONNAIRE")]
[Area("Admin")]
public class UsersController : Controller
{
    private readonly IUtilisateurRepository _users;
    private readonly IAuthService _auth;

    public UsersController(IUtilisateurRepository users, IAuthService auth)
    {
        _users = users;
        _auth = auth;
    }

    public async Task<IActionResult> Pending()
    {
        var pending = await _users.GetPendingValidationAsync();
        return View(pending);
    }

    [HttpPost]
    public async Task<IActionResult> Validate(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (ok, error) = await _auth.ValidateUserAsync(id, currentUserId);
        
        if (!ok)
        {
            TempData["Error"] = error;
        }
        else
        {
            TempData["Success"] = "Utilisateur validé avec succès.";
        }

        return RedirectToAction(nameof(Pending));
    }

    [HttpPost]
    public async Task<IActionResult> Reject(Guid id, string? reason)
    {
        var user = await _users.GetByIdAsync(id);
        if (user != null)
        {
            // Optionnel : envoyer email de rejet
            await _users.DeleteAsync(id);
            await _users.SaveAsync();
            TempData["Success"] = "Inscription rejetée.";
        }
        return RedirectToAction(nameof(Pending));
    }
}
