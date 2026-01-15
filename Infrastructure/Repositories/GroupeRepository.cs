using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class GroupeRepository : IGroupeRepository
{
    private readonly AppDbContext _db;
    public GroupeRepository(AppDbContext db) => _db = db;

    public Task<List<Groupe>> GetAllAsync() => _db.Groupes.OrderBy(x => x.Nom).ToListAsync();
    public Task<Groupe?> GetByIdAsync(Guid id) => _db.Groupes.FirstOrDefaultAsync(x => x.Id == id);

    public async Task AddAsync(Groupe g) => await _db.Groupes.AddAsync(g);

    public Task DeleteAsync(Groupe g)
    {
        _db.Groupes.Remove(g);
        return Task.CompletedTask;
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}