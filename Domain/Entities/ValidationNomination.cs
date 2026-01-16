using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Domain.Entities;

public class ValidationNomination
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid NominationId { get; set; }
    public Nomination Nomination { get; set; } = default!;

    public int StepOrder { get; set; }

    public Guid ValideurId { get; set; }
    public Utilisateur Valideur { get; set; } = default!;

    public DecisionValidation Decision { get; set; }
    public string? Commentaire { get; set; }
    public DateTime DecidedAt { get; set; } = DateTime.UtcNow;
}
