using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface INominationRepository
{
    Task<List<Nomination>> GetAllAsync();
    Task<List<Nomination>> GetByScoutIdAsync(Guid scoutId);
    Task<List<Nomination>> GetByGroupeIdAsync(Guid groupeId);
    Task<List<Nomination>> GetFilteredAsync(Guid? groupeId, Guid? scoutId, DateTime? start, DateTime? end);
    Task<Nomination?> GetByIdAsync(Guid id);
    Task AddAsync(Nomination n);
    Task DeleteAsync(Nomination n);
    Task SaveAsync();
}
