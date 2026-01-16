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

    public async Task<List<Utilisateur>> GetPendingValidationAsync()
    {
        return await _db.Utilisateurs
            .Where(u => !u.IsValidated && !u.IsActive)
            .Include(u => u.UtilisateurRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Utilisateur user) => await _db.Utilisateurs.AddAsync(user);

    public Task UpdateAsync(Utilisateur user)
    {
        _db.Utilisateurs.Update(user);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            _db.Utilisateurs.Remove(user);
        }
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
