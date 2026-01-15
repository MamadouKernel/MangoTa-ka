namespace MangoTaikaDistrict.Domain.Entities;

public class Unite
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid GroupeId { get; set; }
    public Groupe Groupe { get; set; } = default!;

    public string Branche { get; set; } = default!;
    public string? Nom { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Scout> Scouts { get; set; } = new();
}
