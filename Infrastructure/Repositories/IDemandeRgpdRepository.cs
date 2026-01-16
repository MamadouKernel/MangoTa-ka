using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface IDemandeRgpdRepository
{
    Task<List<DemandeDroitRgpd>> GetByUtilisateurAsync(Guid utilisateurId);
    Task<List<DemandeDroitRgpd>> GetPendingAsync();
    Task<DemandeDroitRgpd?> GetByIdAsync(Guid id);
    Task AddAsync(DemandeDroitRgpd demande);
    Task UpdateAsync(DemandeDroitRgpd demande);
    Task SaveAsync();
}
