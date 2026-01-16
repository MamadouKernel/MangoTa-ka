using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Application.Interfaces;
using MangoTaikaDistrict.Models.Auth;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Controllers.Account;

public class AccountController : Controller
{
    private readonly IAuthService _auth;
    private readonly AppDbContext _db;

    public AccountController(IAuthService auth, AppDbContext db)
    {
        _auth = auth;
        _db = db;
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginVm());

    [HttpPost]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var (ok, error, requiresMfa) = await _auth.LoginAsync(vm.Telephone, vm.Password, HttpContext);
        
        if (requiresMfa)
        {
            return RedirectToAction(nameof(MfaVerify));
        }

        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "Erreur de connexion.");
            return View(vm);
        }

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    [HttpGet]
    public IActionResult MfaVerify() => View(new MfaVerifyVm());

    [HttpPost]
    public async Task<IActionResult> MfaVerify(MfaVerifyVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var (ok, error) = await _auth.VerifyMfaAsync(vm.Code, HttpContext);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "Code invalide.");
            return View(vm);
        }

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterVm());

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (!vm.Consentement)
        {
            ModelState.AddModelError(nameof(vm.Consentement), "Vous devez accepter le consentement pour continuer.");
            return View(vm);
        }

        // Enregistrer le consentement RGPD
        var consentement = new Consentement
        {
            UtilisateurId = Guid.Empty, // Sera mis à jour après création
            Version = "v1",
            AcceptedAt = DateTime.UtcNow,
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
        };
        await _db.Consentements.AddAsync(consentement);

        var (ok, error) = await _auth.RegisterAsync(vm.Telephone, vm.Nom, vm.Prenoms, vm.Email, vm.Role, vm.Password);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "Erreur lors de l'inscription.");
            return View(vm);
        }

        await _db.SaveChangesAsync();

        TempData["Success"] = "Votre inscription a été enregistrée. Un administrateur validera votre compte sous peu. Vous recevrez une notification par email.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _auth.LogoutAsync(HttpContext);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();
}
