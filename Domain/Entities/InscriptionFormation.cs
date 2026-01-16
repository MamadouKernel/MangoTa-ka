using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Domain.Entities;

/// <summary>
/// Inscription d'un scout à une formation
/// </summary>
public class InscriptionFormation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid FormationId { get; set; }
    public Formation Formation { get; set; } = default!;

    public Guid ScoutId { get; set; }
    public Scout Scout { get; set; } = default!;

    public StatutInscription Statut { get; set; } = StatutInscription.INSCRIT;
    public DateTime DateInscription { get; set; } = DateTime.UtcNow;
    public DateTime? DateDebut { get; set; }
    public DateTime? DateCompletion { get; set; }
    public decimal? NoteFinale { get; set; } // Sur 20 ou 100
    public string? Commentaire { get; set; }

    // Relations
    public List<ProgressionModule> Progressions { get; set; } = new();
    public List<Certificat> Certificats { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
