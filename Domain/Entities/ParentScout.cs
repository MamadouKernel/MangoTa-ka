namespace MangoTaikaDistrict.Domain.Entities;

public class ParentScout
{
    public Guid ParentId { get; set; }
    public Parent Parent { get; set; } = default!;

    public Guid ScoutId { get; set; }
    public Scout Scout { get; set; } = default!;

    public string Lien { get; set; } = "Tuteur";
}
