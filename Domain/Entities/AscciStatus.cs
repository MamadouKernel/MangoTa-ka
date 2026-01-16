namespace MangoTaikaDistrict.Domain.Entities;

/// <summary>
/// Statut ASCCI d'un scout (vérification manuelle ou via API)
/// </summary>
public class AscciStatus
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ScoutId { get; set; }
    public Scout Scout { get; set; } = default!;

    public string? NumeroCarte { get; set; }
    public string Statut { get; set; } = "Non vérifié"; // Valide, Expiré, Non vérifié, Invalide
    public DateTime? DateVerification { get; set; }
    public DateTime? DateExpiration { get; set; }
    public string? Observations { get; set; }

    public Guid? VerifieParId { get; set; }
    public Utilisateur? VerifiePar { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
