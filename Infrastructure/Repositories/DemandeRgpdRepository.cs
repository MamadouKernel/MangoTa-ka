using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class DemandeRgpdRepository : IDemandeRgpdRepository
{
    private readonly AppDbContext _db;
    public DemandeRgpdRepository(AppDbContext db) => _db = db;

    public Task<List<DemandeDroitRgpd>> GetByUtilisateurAsync(Guid utilisateurId) =>
        _db.DemandesDroitRgpd
            .Where(d => d.UtilisateurId == utilisateurId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();

    public Task<List<DemandeDroitRgpd>> GetPendingAsync() =>
        _db.DemandesDroitRgpd
            .Where(d => d.Statut == StatutDemandeRgpd.EN_ATTENTE || d.Statut == StatutDemandeRgpd.EN_COURS)
            .Include(d => d.Utilisateur)
            .Include(d => d.TraitePar)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();

    public Task<DemandeDroitRgpd?> GetByIdAsync(Guid id) =>
        _db.DemandesDroitRgpd
            .Include(d => d.Utilisateur)
            .Include(d => d.TraitePar)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task AddAsync(DemandeDroitRgpd demande) => await _db.DemandesDroitRgpd.AddAsync(demande);

    public Task UpdateAsync(DemandeDroitRgpd demande)
    {
        _db.DemandesDroitRgpd.Update(demande);
        return Task.CompletedTask;
    }

    public Task SaveAsync() => _db.SaveChangesAsync();
}
