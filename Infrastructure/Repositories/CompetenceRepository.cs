using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class CompetenceRepository : ICompetenceRepository
{
    private readonly AppDbContext _db;
    public CompetenceRepository(AppDbContext db) => _db = db;

    public Task<List<Competence>> GetAllAsync() => 
        _db.Competences.OrderBy(c => c.Type).ThenBy(c => c.Libelle).ToListAsync();

    public Task<List<Competence>> GetByTypeAsync(TypeCompetence type) =>
        _db.Competences.Where(c => c.Type == type).OrderBy(c => c.Libelle).ToListAsync();

    public Task<Competence?> GetByIdAsync(Guid id) =>
        _db.Competences.FirstOrDefaultAsync(c => c.Id == id);

    public Task<List<ScoutCompetence>> GetScoutCompetencesAsync(Guid scoutId) =>
        _db.ScoutCompetences
            .Include(sc => sc.Competence)
            .Where(sc => sc.ScoutId == scoutId)
            .OrderBy(sc => sc.Competence.Type)
            .ThenBy(sc => sc.Competence.Libelle)
            .ToListAsync();

    public async Task AddAsync(Competence c) => await _db.Competences.AddAsync(c);

    public async Task AddScoutCompetenceAsync(ScoutCompetence sc) => await _db.ScoutCompetences.AddAsync(sc);

    public Task UpdateAsync(Competence c)
    {
        _db.Competences.Update(c);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Competence c)
    {
        _db.Competences.Remove(c);
        return Task.CompletedTask;
    }

    public async Task DeleteScoutCompetenceAsync(Guid id)
    {
        var sc = await _db.ScoutCompetences.FirstOrDefaultAsync(x => x.Id == id);
        if (sc != null)
        {
            _db.ScoutCompetences.Remove(sc);
        }
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
