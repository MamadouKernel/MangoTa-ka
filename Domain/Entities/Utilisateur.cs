namespace MangoTaikaDistrict.Domain.Entities;

public class Utilisateur
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Telephone { get; set; } = default!;
    public string? Email { get; set; }

    public string Nom { get; set; } = default!;
    public string Prenoms { get; set; } = default!;

    public string PasswordHash { get; set; } = default!;
    public bool IsActive { get; set; } = false; // Changé à false par défaut pour validation admin

    public bool MfaEnabled { get; set; } = false;
    public string? MfaSecret { get; set; } // Secret TOTP pour MFA

    // Statut de validation pour inscription publique
    public bool IsValidated { get; set; } = false;
    public Guid? ValidatedById { get; set; }
    public Utilisateur? ValidatedBy { get; set; }
    public DateTime? ValidatedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<UtilisateurRole> UtilisateurRoles { get; set; } = new();
    public List<Scout> Scouts { get; set; } = new(); // Un utilisateur peut être lié à plusieurs scouts (ex: parent)
    public List<DemandeDroitRgpd> DemandesDroitRgpd { get; set; } = new();
}
