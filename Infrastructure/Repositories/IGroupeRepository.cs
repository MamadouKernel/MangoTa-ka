using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface IGroupeRepository
{
    Task<List<Groupe>> GetAllAsync();
    Task<Groupe?> GetByIdAsync(Guid id);
    Task AddAsync(Groupe g);
    Task DeleteAsync(Groupe g);
    Task SaveAsync();
}