using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface IUtilisateurRepository
{
    Task<Utilisateur?> GetByTelephoneAsync(string telephone);
    Task<Utilisateur?> GetByIdAsync(Guid id);
    Task<List<string>> GetRoleCodesAsync(Guid utilisateurId);
    Task<List<Utilisateur>> GetPendingValidationAsync();
    Task<List<Utilisateur>> GetAllValidatedAsync();
    Task AddAsync(Utilisateur user);
    Task UpdateAsync(Utilisateur user);
    Task DeleteAsync(Guid id);
    Task SaveAsync();
}
