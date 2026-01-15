using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface IScoutRepository
{
    Task<List<Scout>> GetAllAsync();
    Task<Scout?> GetByIdAsync(Guid id);
    Task AddAsync(Scout s);
    Task DeleteAsync(Scout s);
    Task SaveAsync();
}