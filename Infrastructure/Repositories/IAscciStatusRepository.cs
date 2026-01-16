using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface IAscciStatusRepository
{
    Task<AscciStatus?> GetByScoutIdAsync(Guid scoutId);
    Task<List<AscciStatus>> GetAllAsync();
    Task<List<AscciStatus>> GetExpiredAsync();
    Task<List<AscciStatus>> GetExpiringSoonAsync(int daysAhead = 30);
    Task AddAsync(AscciStatus status);
    Task UpdateAsync(AscciStatus status);
    Task SaveAsync();
}
