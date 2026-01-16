namespace MangoTaikaDistrict.Application.Interfaces;

public interface IAuthService
{
    Task<(bool ok, string? error, bool requiresMfa)> LoginAsync(string telephone, string password, HttpContext httpContext);
    Task<(bool ok, string? error)> VerifyMfaAsync(string code, HttpContext httpContext);
    Task LogoutAsync(HttpContext httpContext);
    Task<(bool ok, string? error)> RegisterAsync(string telephone, string nom, string prenoms, string? email, string role, string password);
    Task<(bool ok, string? error)> ValidateUserAsync(Guid userId, Guid validatedById);
    Task<(string secret, string qrCodeUri)> SetupMfaAsync(Guid userId);
    Task<(bool ok, string? error)> EnableMfaAsync(Guid userId, string code);
    Task<(bool ok, string? error)> DisableMfaAsync(Guid userId);
}
