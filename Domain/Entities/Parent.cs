namespace MangoTaikaDistrict.Domain.Entities;

public class Parent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ParentScout> ParentScouts { get; set; } = new();
}
