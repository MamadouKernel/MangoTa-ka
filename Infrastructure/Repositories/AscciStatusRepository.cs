using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class AscciStatusRepository : IAscciStatusRepository
{
    private readonly AppDbContext _db;
    public AscciStatusRepository(AppDbContext db) => _db = db;

    public Task<AscciStatus?> GetByScoutIdAsync(Guid scoutId) =>
        _db.AscciStatuses
            .Include(a => a.Scout)
            .Include(a => a.VerifiePar)
            .FirstOrDefaultAsync(a => a.ScoutId == scoutId);

    public Task<List<AscciStatus>> GetAllAsync() =>
        _db.AscciStatuses
            .Include(a => a.Scout)
            .Include(a => a.VerifiePar)
            .OrderByDescending(a => a.DateVerification)
            .ToListAsync();

    public Task<List<AscciStatus>> GetExpiredAsync()
    {
        var today = DateTime.UtcNow.Date;
        return _db.AscciStatuses
            .Include(a => a.Scout)
            .Where(a => a.DateExpiration.HasValue && a.DateExpiration.Value.Date < today)
            .ToListAsync();
    }

    public Task<List<AscciStatus>> GetExpiringSoonAsync(int daysAhead = 30)
    {
        var today = DateTime.UtcNow.Date;
        var targetDate = DateTime.UtcNow.AddDays(daysAhead).Date;
        return _db.AscciStatuses
            .Include(a => a.Scout)
            .Where(a => a.DateExpiration.HasValue 
                && a.DateExpiration.Value.Date >= today
                && a.DateExpiration.Value.Date <= targetDate)
            .ToListAsync();
    }

    public async Task AddAsync(AscciStatus status) => await _db.AscciStatuses.AddAsync(status);

    public Task UpdateAsync(AscciStatus status)
    {
        _db.AscciStatuses.Update(status);
        return Task.CompletedTask;
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
