namespace MangoTaikaDistrict.Domain.Entities;

public class Consentement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; } = default!;

    public string Version { get; set; } = "v1";
    public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;

    public string? Ip { get; set; }
    public string? UserAgent { get; set; }
}
