namespace MangoTaikaDistrict.Domain.Entities;

/// <summary>
/// Module d'une formation (leçon, chapitre)
/// </summary>
public class ModuleFormation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid FormationId { get; set; }
    public Formation Formation { get; set; } = default!;

    public string Titre { get; set; } = default!;
    public string? Description { get; set; }
    public string? Contenu { get; set; } // Contenu HTML ou markdown
    public string? VideoUrl { get; set; }
    public string? DocumentUrl { get; set; }
    public int DureeEstimee { get; set; } // En minutes
    public int Ordre { get; set; } = 0;
    public bool EstObligatoire { get; set; } = true;

    // Relations
    public List<ProgressionModule> Progressions { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
