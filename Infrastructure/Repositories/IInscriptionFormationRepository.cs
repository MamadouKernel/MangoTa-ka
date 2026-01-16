using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface IInscriptionFormationRepository
{
    Task<List<InscriptionFormation>> GetByScoutIdAsync(Guid scoutId);
    Task<List<InscriptionFormation>> GetByFormationIdAsync(Guid formationId);
    Task<InscriptionFormation?> GetByIdAsync(Guid id);
    Task<InscriptionFormation?> GetByIdWithDetailsAsync(Guid id);
    Task<InscriptionFormation?> GetByScoutAndFormationAsync(Guid scoutId, Guid formationId);
    Task AddAsync(InscriptionFormation inscription);
    Task UpdateAsync(InscriptionFormation inscription);
    Task DeleteAsync(Guid id);
    Task SaveAsync();
}
