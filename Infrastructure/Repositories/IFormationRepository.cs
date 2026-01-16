using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface IFormationRepository
{
    Task<List<Formation>> GetAllAsync(bool includeInactive = false);
    Task<List<Formation>> GetPublicAsync();
    Task<Formation?> GetByIdAsync(Guid id);
    Task<Formation?> GetByIdWithModulesAsync(Guid id);
    Task AddAsync(Formation formation);
    Task UpdateAsync(Formation formation);
    Task DeleteAsync(Guid id);
    Task SaveAsync();
}
