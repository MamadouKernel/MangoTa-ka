using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class InscriptionFormationRepository : IInscriptionFormationRepository
{
    private readonly AppDbContext _db;
    public InscriptionFormationRepository(AppDbContext db) => _db = db;

    public Task<List<InscriptionFormation>> GetByScoutIdAsync(Guid scoutId) =>
        _db.InscriptionsFormation
            .Include(i => i.Formation)
            .Include(i => i.Scout)
            .Where(i => i.ScoutId == scoutId)
            .OrderByDescending(i => i.DateInscription)
            .ToListAsync();

    public Task<List<InscriptionFormation>> GetByFormationIdAsync(Guid formationId) =>
        _db.InscriptionsFormation
            .Include(i => i.Scout)
            .Where(i => i.FormationId == formationId)
            .OrderByDescending(i => i.DateInscription)
            .ToListAsync();

    public Task<InscriptionFormation?> GetByIdAsync(Guid id) =>
        _db.InscriptionsFormation
            .Include(i => i.Formation)
            .Include(i => i.Scout)
            .FirstOrDefaultAsync(i => i.Id == id);

    public Task<InscriptionFormation?> GetByIdWithDetailsAsync(Guid id) =>
        _db.InscriptionsFormation
            .Include(i => i.Formation)
                .ThenInclude(f => f.Modules.OrderBy(m => m.Ordre))
            .Include(i => i.Scout)
            .Include(i => i.Progressions)
                .ThenInclude(p => p.ModuleFormation)
            .Include(i => i.Certificats)
            .FirstOrDefaultAsync(i => i.Id == id);

    public Task<InscriptionFormation?> GetByScoutAndFormationAsync(Guid scoutId, Guid formationId) =>
        _db.InscriptionsFormation
            .Include(i => i.Formation)
            .Include(i => i.Scout)
            .FirstOrDefaultAsync(i => i.ScoutId == scoutId && i.FormationId == formationId);

    public async Task AddAsync(InscriptionFormation inscription) => await _db.InscriptionsFormation.AddAsync(inscription);

    public Task UpdateAsync(InscriptionFormation inscription)
    {
        _db.InscriptionsFormation.Update(inscription);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var inscription = await GetByIdAsync(id);
        if (inscription != null)
            _db.InscriptionsFormation.Remove(inscription);
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
