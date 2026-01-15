using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Application.Interfaces;
using MangoTaikaDistrict.Models.Auth;

namespace MangoTaikaDistrict.Controllers.Account;

public class AccountController : Controller
{
    private readonly IAuthService _auth;
    public AccountController(IAuthService auth) => _auth = auth;

    [HttpGet]
    public IActionResult Login() => View(new LoginVm());

    [HttpPost]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var (ok, error) = await _auth.LoginAsync(vm.Telephone, vm.Password, HttpContext);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "Erreur de connexion.");
            return View(vm);
        }

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
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
