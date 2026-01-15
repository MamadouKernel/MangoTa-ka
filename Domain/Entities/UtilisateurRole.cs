namespace MangoTaikaDistrict.Domain.Entities;

public class UtilisateurRole
{
    public Guid UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; } = default!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;
}
