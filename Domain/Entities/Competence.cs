using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Domain.Entities;

public class Competence
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Libelle { get; set; } = default!;
    public string Description { get; set; } = default!;
    public TypeCompetence Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<ScoutCompetence> ScoutCompetences { get; set; } = new();
}
