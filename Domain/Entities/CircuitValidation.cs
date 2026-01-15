namespace MangoTaikaDistrict.Domain.Entities;

public class CircuitValidation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = default!;        // ex: AUTORISATION_ACTIVITE
    public string Libelle { get; set; } = default!;
    public bool IsActive { get; set; } = true;

    public List<CircuitEtape> Etapes { get; set; } = new();
}
