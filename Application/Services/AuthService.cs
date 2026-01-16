using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MangoTaikaDistrict.Application.Interfaces;
using MangoTaikaDistrict.Domain.Enums;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Infrastructure.Security;

namespace MangoTaikaDistrict.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUtilisateurRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IPasswordService _password;
    private readonly IMfaService _mfa;

    public AuthService(IUtilisateurRepository users, IRoleRepository roles, IPasswordService password, IMfaService mfa)
    {
        _users = users;
        _roles = roles;
        _password = password;
        _mfa = mfa;
    }

    public async Task<(bool ok, string? error, bool requiresMfa)> LoginAsync(string telephone, string password, HttpContext httpContext)
    {
        var user = await _users.GetByTelephoneAsync(telephone);
        if (user is null || !user.IsActive) return (false, "Compte introuvable ou désactivé.", false);

        if (!_password.Verify(password, user.PasswordHash))
            return (false, "Téléphone ou mot de passe incorrect.", false);

        // Si MFA activé, retourner requiresMfa = true
        if (user.MfaEnabled && !string.IsNullOrEmpty(user.MfaSecret))
        {
            // Stocker temporairement l'ID utilisateur pour la vérification MFA
            httpContext.Session.SetString("MfaUserId", user.Id.ToString());
            return (false, null, true);
        }

        // Connexion normale
        await SignInUserAsync(user, httpContext);
        return (true, null, false);
    }

    public async Task<(bool ok, string? error)> VerifyMfaAsync(string code, HttpContext httpContext)
    {
        var userIdStr = httpContext.Session.GetString("MfaUserId");
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return (false, "Session expirée.");

        var user = await _users.GetByIdAsync(userId);
        if (user == null || string.IsNullOrEmpty(user.MfaSecret))
            return (false, "Utilisateur introuvable ou MFA non configuré.");

        if (!_mfa.ValidateCode(user.MfaSecret, code))
            return (false, "Code invalide.");

        // Connexion réussie
        await SignInUserAsync(user, httpContext);
        httpContext.Session.Remove("MfaUserId");
        return (true, null);
    }

    private async Task SignInUserAsync(Utilisateur user, HttpContext httpContext)
    {
        var roleCodes = await _users.GetRoleCodesAsync(user.Id);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.Nom} {user.Prenoms}"),
            new("telephone", user.Telephone)
        };

        foreach (var rc in roleCodes)
            claims.Add(new Claim(ClaimTypes.Role, rc));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    public async Task LogoutAsync(HttpContext httpContext)
        => await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    public async Task<(bool ok, string? error)> RegisterAsync(string telephone, string nom, string prenoms, string? email, string role, string password)
    {
        var existing = await _users.GetByTelephoneAsync(telephone);
        if (existing != null) return (false, "Ce téléphone est déjà utilisé.");

        var user = new Utilisateur
        {
            Telephone = telephone,
            Nom = nom,
            Prenoms = prenoms,
            Email = email,
            PasswordHash = _password.Hash(password),
            IsActive = false, // En attente de validation
            IsValidated = false
        };

        await _users.AddAsync(user);
        await _users.SaveAsync();

        // Assigner le rôle demandé (sera validé par admin)
        var roleEnum = Enum.TryParse<RoleCode>(role, out var roleCode) ? roleCode : RoleCode.CONSULTANT;
        var roleEntity = await _roles.GetByCodeAsync(roleEnum);
        if (roleEntity != null)
        {
            await _roles.AddUserRoleAsync(user.Id, roleEntity.Id);
            await _roles.SaveAsync();
        }

        return (true, null);
    }

    public async Task<(bool ok, string? error)> ValidateUserAsync(Guid userId, Guid validatedById)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user == null) return (false, "Utilisateur introuvable.");

        user.IsActive = true;
        user.IsValidated = true;
        user.ValidatedById = validatedById;
        user.ValidatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _users.UpdateAsync(user);
        await _users.SaveAsync();
        return (true, null);
    }

    public async Task<(string secret, string qrCodeUri)> SetupMfaAsync(Guid userId)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user == null) throw new Exception("Utilisateur introuvable.");

        var secret = _mfa.GenerateSecret();
        user.MfaSecret = secret;
        await _users.UpdateAsync(user);
        await _users.SaveAsync();

        var email = user.Email ?? user.Telephone;
        var qrCodeUri = _mfa.GenerateQrCodeUri(email, secret);
        return (secret, qrCodeUri);
    }

    public async Task<(bool ok, string? error)> EnableMfaAsync(Guid userId, string code)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user == null || string.IsNullOrEmpty(user.MfaSecret))
            return (false, "Utilisateur introuvable ou MFA non configuré.");

        if (!_mfa.ValidateCode(user.MfaSecret, code))
            return (false, "Code invalide.");

        user.MfaEnabled = true;
        await _users.UpdateAsync(user);
        await _users.SaveAsync();
        return (true, null);
    }

    public async Task<(bool ok, string? error)> DisableMfaAsync(Guid userId)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user == null) return (false, "Utilisateur introuvable.");

        user.MfaEnabled = false;
        user.MfaSecret = null;
        await _users.UpdateAsync(user);
        await _users.SaveAsync();
        return (true, null);
    }
}
