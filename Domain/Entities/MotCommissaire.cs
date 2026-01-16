namespace MangoTaikaDistrict.Domain.Entities;

public class MotCommissaire
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Titre { get; set; } = default!;
    public string Contenu { get; set; } = default!;
    public string? PhotoUrl { get; set; }

    public int Annee { get; set; } // Année scoute (ex: 2025-2026)
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid CreatedById { get; set; }
    public Utilisateur CreatedBy { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
