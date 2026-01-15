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
        _db.Scouts.Include(s => s.Groupe).FirstOrDefaultAsync(x => x.Id == id);

    public async Task AddAsync(Scout s) => await _db.Scouts.AddAsync(s);

    public Task DeleteAsync(Scout s)
    {
        _db.Scouts.Remove(s);
        return Task.CompletedTask;
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
