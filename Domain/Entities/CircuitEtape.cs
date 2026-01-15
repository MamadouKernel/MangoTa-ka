namespace MangoTaikaDistrict.Domain.Entities;

public class CircuitEtape
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CircuitValidationId { get; set; }
    public CircuitValidation CircuitValidation { get; set; } = default!;

    public int StepOrder { get; set; }
    public string RoleRequis { get; set; } = default!;  // SUPERVISEUR, ADMIN...
    public string Libelle { get; set; } = default!;
}
