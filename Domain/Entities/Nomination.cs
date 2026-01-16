using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Domain.Entities;

public class Nomination
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ScoutId { get; set; }
    public Scout Scout { get; set; } = default!;

    public Guid GroupeId { get; set; }
    public Groupe Groupe { get; set; } = default!;

    public string Poste { get; set; } = default!;
    public string? Fonction { get; set; }

    public DateOnly DateDebut { get; set; }
    public DateOnly? DateFin { get; set; }

    public StatutNomination Statut { get; set; } = StatutNomination.BROUILLON;
    public int CurrentStep { get; set; } = 1;

    public Guid? AutoriteValidationId { get; set; }
    public Utilisateur? AutoriteValidation { get; set; }

    public Guid? CreeParId { get; set; }
    public Utilisateur? CreePar { get; set; }

    public string? Observations { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<ValidationNomination> Validations { get; set; } = new();
}
