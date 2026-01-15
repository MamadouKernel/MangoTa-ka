using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Domain.Entities;

public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public RoleCode Code { get; set; }
    public string Libelle { get; set; } = default!;
}
