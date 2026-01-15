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

    public AuthService(IUtilisateurRepository users, IRoleRepository roles, IPasswordService password)
    {
        _users = users;
        _roles = roles;
        _password = password;
    }

    public async Task<(bool ok, string? error)> LoginAsync(string telephone, string password, HttpContext httpContext)
    {
        var user = await _users.GetByTelephoneAsync(telephone);
        if (user is null || !user.IsActive) return (false, "Compte introuvable ou désactivé.");

        if (!_password.Verify(password, user.PasswordHash))
            return (false, "Téléphone ou mot de passe incorrect.");

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

        return (true, null);
    }

    public async Task LogoutAsync(HttpContext httpContext)
        => await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    public async Task<(bool ok, string? error)> RegisterAsync(string telephone, string nom, string prenoms, string password)
    {
        var existing = await _users.GetByTelephoneAsync(telephone);
        if (existing != null) return (false, "Ce téléphone est déjà utilisé.");

        var user = new Utilisateur
        {
            Telephone = telephone,
            Nom = nom,
            Prenoms = prenoms,
            PasswordHash = _password.Hash(password),
            IsActive = true
        };

        await _users.AddAsync(user);
        await _users.SaveAsync();

        // Par défaut : CONSULTANT (tu peux changer en SCOUT ou PARENT selon ton besoin)
        var role = await _roles.GetByCodeAsync(RoleCode.CONSULTANT);
        if (role != null)
        {
            await _roles.AddUserRoleAsync(user.Id, role.Id);
            await _roles.SaveAsync();
        }

        return (true, null);
    }
}
