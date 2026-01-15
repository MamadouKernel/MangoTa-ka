namespace MangoTaikaDistrict.Domain.Entities;

public class Utilisateur
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Telephone { get; set; } = default!;
    public string? Email { get; set; }

    public string Nom { get; set; } = default!;
    public string Prenoms { get; set; } = default!;

    public string PasswordHash { get; set; } = default!;
    public bool IsActive { get; set; } = true;

    public bool MfaEnabled { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<UtilisateurRole> UtilisateurRoles { get; set; } = new();
}
