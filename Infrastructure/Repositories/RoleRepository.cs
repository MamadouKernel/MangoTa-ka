using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _db;
    public RoleRepository(AppDbContext db) => _db = db;

    public Task<Role?> GetByCodeAsync(RoleCode code) =>
        _db.Roles.FirstOrDefaultAsync(r => r.Code == code);

    public async Task AddUserRoleAsync(Guid userId, Guid roleId)
        => await _db.UtilisateurRoles.AddAsync(new UtilisateurRole { UtilisateurId = userId, RoleId = roleId });

    public Task SaveAsync() => _db.SaveChangesAsync();
}
