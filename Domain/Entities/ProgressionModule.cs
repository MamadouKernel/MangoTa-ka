namespace MangoTaikaDistrict.Domain.Entities;

/// <summary>
/// Progression d'un scout sur un module de formation
/// </summary>
public class ProgressionModule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InscriptionFormationId { get; set; }
    public InscriptionFormation InscriptionFormation { get; set; } = default!;

    public Guid ModuleFormationId { get; set; }
    public ModuleFormation ModuleFormation { get; set; } = default!;

    public bool EstComplete { get; set; } = false;
    public DateTime? DateDebut { get; set; }
    public DateTime? DateCompletion { get; set; }
    public int TempsConsacre { get; set; } = 0; // En minutes
    public decimal? Note { get; set; } // Note sur le module si évaluation
    public string? Commentaire { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
