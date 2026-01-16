using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class ParentRepository : IParentRepository
{
    private readonly AppDbContext _db;
    public ParentRepository(AppDbContext db) => _db = db;

    public Task<Parent?> GetByUtilisateurIdAsync(Guid utilisateurId) =>
        _db.Parents
            .Include(p => p.ParentScouts)
                .ThenInclude(ps => ps.Scout)
                    .ThenInclude(s => s.Groupe)
            .Include(p => p.ParentScouts)
                .ThenInclude(ps => ps.Scout)
                    .ThenInclude(s => s.Unite)
            .FirstOrDefaultAsync(p => p.UtilisateurId == utilisateurId);

    public async Task<List<Scout>> GetEnfantsAsync(Guid utilisateurId)
    {
        var parent = await GetByUtilisateurIdAsync(utilisateurId);
        if (parent == null) return new List<Scout>();

        return parent.ParentScouts
            .Select(ps => ps.Scout)
            .OrderBy(s => s.Nom)
            .ToList();
    }

    public async Task AddAsync(Parent parent) => await _db.Parents.AddAsync(parent);

    public Task SaveAsync() => _db.SaveChangesAsync();
}
