using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class FormationRepository : IFormationRepository
{
    private readonly AppDbContext _db;
    public FormationRepository(AppDbContext db) => _db = db;

    public Task<List<Formation>> GetAllAsync(bool includeInactive = false) =>
        _db.Formations
            .Include(f => f.CreatedBy)
            .Where(f => includeInactive || f.EstActive)
            .OrderBy(f => f.OrdreAffichage)
            .ThenBy(f => f.Titre)
            .ToListAsync();

    public Task<List<Formation>> GetPublicAsync() =>
        _db.Formations
            .Where(f => f.EstActive && f.EstPublique)
            .OrderBy(f => f.OrdreAffichage)
            .ThenBy(f => f.Titre)
            .ToListAsync();

    public Task<Formation?> GetByIdAsync(Guid id) =>
        _db.Formations
            .Include(f => f.CreatedBy)
            .FirstOrDefaultAsync(f => f.Id == id);

    public Task<Formation?> GetByIdWithModulesAsync(Guid id) =>
        _db.Formations
            .Include(f => f.Modules.OrderBy(m => m.Ordre))
            .Include(f => f.CreatedBy)
            .FirstOrDefaultAsync(f => f.Id == id);

    public async Task AddAsync(Formation formation) => await _db.Formations.AddAsync(formation);

    public Task UpdateAsync(Formation formation)
    {
        _db.Formations.Update(formation);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var formation = await GetByIdAsync(id);
        if (formation != null)
            _db.Formations.Remove(formation);
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
