using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface ICotisationRepository
{
    Task<List<Cotisation>> GetAllAsync();
    Task<List<Cotisation>> GetByScoutIdAsync(Guid scoutId);
    Task<List<Cotisation>> GetByGroupeIdAsync(Guid groupeId);
    Task<List<Cotisation>> GetByPeriodeAsync(string periode);
    Task<List<Cotisation>> GetFilteredAsync(Guid? groupeId, string? periode, DateTime? start, DateTime? end);
    Task<Cotisation?> GetByIdAsync(Guid id);
    Task AddAsync(Cotisation c);
    Task DeleteAsync(Cotisation c);
    Task SaveAsync();
}
