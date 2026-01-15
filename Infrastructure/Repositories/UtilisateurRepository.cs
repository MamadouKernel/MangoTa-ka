using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class UtilisateurRepository : IUtilisateurRepository
{
    private readonly AppDbContext _db;
    public UtilisateurRepository(AppDbContext db) => _db = db;

    public Task<Utilisateur?> GetByTelephoneAsync(string telephone) =>
        _db.Utilisateurs.FirstOrDefaultAsync(u => u.Telephone == telephone);

    public Task<Utilisateur?> GetByIdAsync(Guid id) =>
        _db.Utilisateurs.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<List<string>> GetRoleCodesAsync(Guid utilisateurId)
    {
        return await _db.UtilisateurRoles
            .Where(ur => ur.UtilisateurId == utilisateurId)
            .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Code.ToString())
            .ToListAsync();
    }

    public async Task AddAsync(Utilisateur user) => await _db.Utilisateurs.AddAsync(user);

    public Task SaveAsync() => _db.SaveChangesAsync();
}
