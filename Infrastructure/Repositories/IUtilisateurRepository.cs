using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface IUtilisateurRepository
{
    Task<Utilisateur?> GetByTelephoneAsync(string telephone);
    Task<Utilisateur?> GetByIdAsync(Guid id);
    Task<List<string>> GetRoleCodesAsync(Guid utilisateurId);
    Task AddAsync(Utilisateur user);
    Task SaveAsync();
}
