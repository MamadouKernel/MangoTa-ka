using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface IParentRepository
{
    Task<Parent?> GetByUtilisateurIdAsync(Guid utilisateurId);
    Task<List<Scout>> GetEnfantsAsync(Guid utilisateurId);
    Task AddAsync(Parent parent);
    Task SaveAsync();
}
