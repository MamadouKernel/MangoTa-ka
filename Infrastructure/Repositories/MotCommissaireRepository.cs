using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class MotCommissaireRepository : IMotCommissaireRepository
{
    private readonly AppDbContext _db;

    public MotCommissaireRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<MotCommissaire?> GetActiveAsync()
        => await _db.Set<MotCommissaire>()
            .Include(x => x.CreatedBy)
            .Where(x => x.IsActive && DateTime.UtcNow >= x.DateDebut && DateTime.UtcNow <= x.DateFin)
            .OrderByDescending(x => x.Annee)
            .FirstOrDefaultAsync();

    public async Task<MotCommissaire?> GetByIdAsync(Guid id)
        => await _db.Set<MotCommissaire>()
            .Include(x => x.CreatedBy)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<MotCommissaire>> GetAllAsync()
        => await _db.Set<MotCommissaire>()
            .Include(x => x.CreatedBy)
            .OrderByDescending(x => x.Annee)
            .ToListAsync();

    public async Task AddAsync(MotCommissaire mot)
    {
        await _db.Set<MotCommissaire>().AddAsync(mot);
    }

    public Task UpdateAsync(MotCommissaire mot)
    {
        _db.Set<MotCommissaire>().Update(mot);
        return Task.CompletedTask;
    }

    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }
}
