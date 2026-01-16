using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Domain.Entities;

public class Cotisation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ScoutId { get; set; }
    public Scout Scout { get; set; } = default!;

    public Guid GroupeId { get; set; }
    public Groupe Groupe { get; set; } = default!;

    // Période : année (ex: 2026) ou format "2026-01" pour mensuel
    public string Periode { get; set; } = default!;

    public decimal MontantAttendu { get; set; }
    public decimal MontantPaye { get; set; }

    public StatutCotisation Statut { get; set; } = StatutCotisation.IMPAYE;

    public DateTime DateEnregistrement { get; set; } = DateTime.UtcNow;
    public DateTime? DatePaiement { get; set; }

    public string? Observations { get; set; }

    public Guid? EnregistreParId { get; set; }
    public Utilisateur? EnregistrePar { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
