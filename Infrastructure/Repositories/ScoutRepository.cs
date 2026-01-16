using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class ScoutRepository : IScoutRepository
{
    private readonly AppDbContext _db;
    public ScoutRepository(AppDbContext db) => _db = db;

    public Task<List<Scout>> GetAllAsync() =>
        _db.Scouts.Include(s => s.Groupe).OrderBy(x => x.Nom).ToListAsync();

    public Task<Scout?> GetByIdAsync(Guid id) =>
        _db.Scouts
            .Include(s => s.Groupe)
            .Include(s => s.Unite)
            .Include(s => s.Utilisateur)
            .FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<Scout>> GetByUtilisateurIdAsync(Guid utilisateurId) =>
        _db.Scouts
            .Where(s => s.UtilisateurId == utilisateurId)
            .Include(s => s.Groupe)
            .Include(s => s.Unite)
            .OrderBy(s => s.Nom)
            .ToListAsync();

    public async Task AddAsync(Scout s) => await _db.Scouts.AddAsync(s);

    public Task UpdateAsync(Scout s)
    {
        _db.Scouts.Update(s);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Scout s)
    {
        _db.Scouts.Remove(s);
        return Task.CompletedTask;
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
