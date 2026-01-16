namespace MangoTaikaDistrict.Domain.Entities;

public class ScoutCompetence
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ScoutId { get; set; }
    public Scout Scout { get; set; } = default!;
    public Guid CompetenceId { get; set; }
    public Competence Competence { get; set; } = default!;
    public string? Niveau { get; set; } // Ex: Débutant, Intermédiaire, Avancé, Expert
    public DateOnly? DateAcquisition { get; set; }
    public string? CertificatUrl { get; set; }
    public string? Observations { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
