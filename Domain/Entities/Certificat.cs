namespace MangoTaikaDistrict.Domain.Entities;

/// <summary>
/// Certificat de complétion d'une formation
/// </summary>
public class Certificat
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InscriptionFormationId { get; set; }
    public InscriptionFormation InscriptionFormation { get; set; } = default!;

    public string NumeroCertificat { get; set; } = default!; // Format: CERT-YYYY-XXXXX
    public DateTime DateEmission { get; set; } = DateTime.UtcNow;
    public DateTime? DateExpiration { get; set; }
    public string? UrlCertificat { get; set; } // URL du PDF généré
    public bool EstValide { get; set; } = true;

    public Guid? EmisParId { get; set; }
    public Utilisateur? EmisPar { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
