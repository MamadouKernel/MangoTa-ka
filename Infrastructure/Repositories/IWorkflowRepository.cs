using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public interface IWorkflowRepository
{
    Task<List<Activite>> GetAllActivitesAsync();
    Task<Activite?> GetActiviteAsync(Guid id);

    Task<List<DemandeAutorisation>> GetDemandesEnAttenteAsync(string roleCode);
    Task<DemandeAutorisation?> GetDemandeAsync(Guid id);

    Task<CircuitValidation?> GetCircuitAsync(string code);

    Task AddActiviteAsync(Activite a);
    Task AddDemandeAsync(DemandeAutorisation d);
    Task AddValidationAsync(Validation v);

    Task SaveAsync();
}
