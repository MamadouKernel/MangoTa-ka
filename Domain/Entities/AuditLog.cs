namespace MangoTaikaDistrict.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UtilisateurId { get; set; }
    public Utilisateur? Utilisateur { get; set; }

    public string Action { get; set; } = default!;
    public string EntityName { get; set; } = default!;
    public Guid? EntityId { get; set; }

    public string? BeforeJson { get; set; }
    public string? AfterJson { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
