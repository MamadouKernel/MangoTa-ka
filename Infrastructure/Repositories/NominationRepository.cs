using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class NominationRepository : INominationRepository
{
    private readonly AppDbContext _db;
    public NominationRepository(AppDbContext db) => _db = db;

    public Task<List<Nomination>> GetAllAsync() =>
        _db.Nominations
            .Include(n => n.Scout)
            .Include(n => n.Groupe)
            .Include(n => n.CreePar)
            .Include(n => n.AutoriteValidation)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public Task<List<Nomination>> GetByScoutIdAsync(Guid scoutId) =>
        _db.Nominations
            .Include(n => n.Scout)
            .Include(n => n.Groupe)
            .Where(n => n.ScoutId == scoutId)
            .OrderByDescending(n => n.DateDebut)
            .ToListAsync();

    public Task<List<Nomination>> GetByGroupeIdAsync(Guid groupeId) =>
        _db.Nominations
            .Include(n => n.Scout)
            .Include(n => n.Groupe)
            .Where(n => n.GroupeId == groupeId)
            .OrderByDescending(n => n.DateDebut)
            .ToListAsync();

    public Task<List<Nomination>> GetFilteredAsync(Guid? groupeId, Guid? scoutId, DateTime? start, DateTime? end)
    {
        var query = _db.Nominations
            .Include(n => n.Scout)
            .Include(n => n.Groupe)
            .Include(n => n.CreePar)
            .Include(n => n.AutoriteValidation)
            .AsQueryable();

        if (groupeId.HasValue)
            query = query.Where(n => n.GroupeId == groupeId.Value);

        if (scoutId.HasValue)
            query = query.Where(n => n.ScoutId == scoutId.Value);

        if (start.HasValue)
            query = query.Where(n => n.DateDebut.ToDateTime(TimeOnly.MinValue) >= start.Value);

        if (end.HasValue)
        {
            var endInclusive = end.Value.Date.AddDays(1);
            query = query.Where(n => n.DateDebut.ToDateTime(TimeOnly.MinValue) < endInclusive);
        }

        return query.OrderByDescending(n => n.CreatedAt).ToListAsync();
    }

    public Task<Nomination?> GetByIdAsync(Guid id) =>
        _db.Nominations
            .Include(n => n.Scout)
            .Include(n => n.Groupe)
            .Include(n => n.CreePar)
            .Include(n => n.AutoriteValidation)
            .Include(n => n.Validations)
                .ThenInclude(v => v.Valideur)
            .FirstOrDefaultAsync(n => n.Id == id);

    public async Task AddAsync(Nomination n) => await _db.Nominations.AddAsync(n);

    public Task DeleteAsync(Nomination n)
    {
        _db.Nominations.Remove(n);
        return Task.CompletedTask;
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
