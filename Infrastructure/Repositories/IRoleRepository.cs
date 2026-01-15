using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByCodeAsync(RoleCode code);
    Task AddUserRoleAsync(Guid userId, Guid roleId);
    Task SaveAsync();
}
