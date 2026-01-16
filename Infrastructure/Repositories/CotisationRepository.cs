using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class CotisationRepository : ICotisationRepository
{
    private readonly AppDbContext _db;
    public CotisationRepository(AppDbContext db) => _db = db;

    public Task<List<Cotisation>> GetAllAsync() =>
        _db.Cotisations
            .Include(c => c.Scout)
            .Include(c => c.Groupe)
            .Include(c => c.EnregistrePar)
            .OrderByDescending(c => c.DateEnregistrement)
            .ToListAsync();

    public Task<List<Cotisation>> GetByScoutIdAsync(Guid scoutId) =>
        _db.Cotisations
            .Include(c => c.Scout)
            .Include(c => c.Groupe)
            .Where(c => c.ScoutId == scoutId)
            .OrderByDescending(c => c.DateEnregistrement)
            .ToListAsync();

    public Task<List<Cotisation>> GetByGroupeIdAsync(Guid groupeId) =>
        _db.Cotisations
            .Include(c => c.Scout)
            .Include(c => c.Groupe)
            .Where(c => c.GroupeId == groupeId)
            .OrderByDescending(c => c.DateEnregistrement)
            .ToListAsync();

    public Task<List<Cotisation>> GetByPeriodeAsync(string periode) =>
        _db.Cotisations
            .Include(c => c.Scout)
            .Include(c => c.Groupe)
            .Where(c => c.Periode == periode)
            .OrderByDescending(c => c.DateEnregistrement)
            .ToListAsync();

    public Task<List<Cotisation>> GetFilteredAsync(Guid? groupeId, string? periode, DateTime? start, DateTime? end)
    {
        var query = _db.Cotisations
            .Include(c => c.Scout)
            .Include(c => c.Groupe)
            .Include(c => c.EnregistrePar)
            .AsQueryable();

        if (groupeId.HasValue)
            query = query.Where(c => c.GroupeId == groupeId.Value);

        if (!string.IsNullOrEmpty(periode))
            query = query.Where(c => c.Periode == periode);

        if (start.HasValue)
            query = query.Where(c => c.DateEnregistrement >= start.Value);

        if (end.HasValue)
        {
            var endInclusive = end.Value.Date.AddDays(1);
            query = query.Where(c => c.DateEnregistrement < endInclusive);
        }

        return query.OrderByDescending(c => c.DateEnregistrement).ToListAsync();
    }

    public Task<Cotisation?> GetByIdAsync(Guid id) =>
        _db.Cotisations
            .Include(c => c.Scout)
            .Include(c => c.Groupe)
            .Include(c => c.EnregistrePar)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task AddAsync(Cotisation c) => await _db.Cotisations.AddAsync(c);

    public Task DeleteAsync(Cotisation c)
    {
        _db.Cotisations.Remove(c);
        return Task.CompletedTask;
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
