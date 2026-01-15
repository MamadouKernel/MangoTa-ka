namespace MangoTaikaDistrict.Domain.Entities;

public class Actualite
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titre { get; set; } = default!;
    public string Contenu { get; set; } = default!;

    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedAt { get; set; }

    public Guid CreatedById { get; set; }
    public Utilisateur CreatedBy { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
