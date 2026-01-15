namespace MangoTaikaDistrict.Domain.Entities;

public class CarteScout
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ScoutId { get; set; }
    public Scout Scout { get; set; } = default!;

    public string Numero { get; set; } = default!;
    public DateOnly DateEmission { get; set; }
    public DateOnly DateExpiration { get; set; }
    public string Statut { get; set; } = "Valide";
}
