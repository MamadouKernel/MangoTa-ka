namespace MangoTaikaDistrict.Domain.Entities;

public class Assurance
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ScoutId { get; set; }
    public Scout Scout { get; set; } = default!;

    public string Compagnie { get; set; } = default!;
    public string Reference { get; set; } = default!;
    public DateOnly DateExpiration { get; set; }
    public string Statut { get; set; } = "Valide";
}