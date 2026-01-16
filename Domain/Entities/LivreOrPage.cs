namespace MangoTaikaDistrict.Domain.Entities;

/// <summary>
/// Pages préremplies du livre d'or avec images d'anciens commissaires, CG, membres du CAD
/// </summary>
public class LivreOrPage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titre { get; set; } = default!;
    public string Contenu { get; set; } = default!;
    public string? PhotoUrl { get; set; }
    public int Ordre { get; set; } // Ordre d'affichage
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
