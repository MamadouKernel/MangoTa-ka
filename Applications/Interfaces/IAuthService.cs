namespace MangoTaikaDistrict.Application.Interfaces;

public interface IAuthService
{
    Task<(bool ok, string? error)> LoginAsync(string telephone, string password, HttpContext httpContext);
    Task LogoutAsync(HttpContext httpContext);

    // optionnel: si tu veux plus tard l'inscription publique
    Task<(bool ok, string? error)> RegisterAsync(string telephone, string nom, string prenoms, string password);
}
