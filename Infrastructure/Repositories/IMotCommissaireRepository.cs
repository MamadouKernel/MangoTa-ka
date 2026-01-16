using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface IMotCommissaireRepository
{
    Task<MotCommissaire?> GetActiveAsync();
    Task<MotCommissaire?> GetByIdAsync(Guid id);
    Task<List<MotCommissaire>> GetAllAsync();
    Task AddAsync(MotCommissaire mot);
    Task UpdateAsync(MotCommissaire mot);
    Task SaveAsync();
}
