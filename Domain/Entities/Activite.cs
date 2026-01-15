using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Domain.Entities;

public class Activite
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid GroupeId { get; set; }
    public Groupe Groupe { get; set; } = default!;

    public string Titre { get; set; } = default!;
    public string TypeActivite { get; set; } = default!;
    public string? Lieu { get; set; }

    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }

    public StatutDemande Statut { get; set; } = StatutDemande.BROUILLON;

    public Guid CreatedById { get; set; }
    public Utilisateur CreatedBy { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DemandeAutorisation? DemandeAutorisation { get; set; }
}
