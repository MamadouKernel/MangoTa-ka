using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class WorkflowRepository : IWorkflowRepository
{
    private readonly AppDbContext _db;
    public WorkflowRepository(AppDbContext db) => _db = db;

    public Task<List<Activite>> GetAllActivitesAsync() =>
        _db.Activites.Include(a => a.Groupe).Include(a => a.DemandeAutorisation)
          .OrderByDescending(a => a.CreatedAt).ToListAsync();

    public Task<Activite?> GetActiviteAsync(Guid id) =>
        _db.Activites.Include(a => a.Groupe)
          .Include(a => a.DemandeAutorisation).ThenInclude(d => d.Validations).ThenInclude(v => v.Valideur)
          .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<List<DemandeAutorisation>> GetDemandesEnAttenteAsync(string roleCode)
    {
        // pending = demande EN_ATTENTE dont current step correspond à une étape role_requis = roleCode
        var circuit = await GetCircuitAsync("AUTORISATION_ACTIVITE");
        if (circuit == null) return new();

        var step = circuit.Etapes.OrderBy(e => e.StepOrder).FirstOrDefault(e => e.RoleRequis == roleCode);
        if (step == null) return new();

        return await _db.DemandesAutorisation
            .Include(d => d.Activite).ThenInclude(a => a.Groupe)
            .Include(d => d.Validations).ThenInclude(v => v.Valideur)
            .Where(d => d.Statut == StatutDemande.EN_ATTENTE && d.CurrentStep == step.StepOrder)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync();
    }

    public Task<DemandeAutorisation?> GetDemandeAsync(Guid id) =>
        _db.DemandesAutorisation
          .Include(d => d.Activite).ThenInclude(a => a.Groupe)
          .Include(d => d.Validations).ThenInclude(v => v.Valideur)
          .FirstOrDefaultAsync(d => d.Id == id);

    public Task<CircuitValidation?> GetCircuitAsync(string code) =>
        _db.CircuitsValidation.Include(c => c.Etapes)
          .FirstOrDefaultAsync(c => c.Code == code && c.IsActive);

    public Task AddActiviteAsync(Activite a) => _db.Activites.AddAsync(a).AsTask();
    public Task AddDemandeAsync(DemandeAutorisation d) => _db.DemandesAutorisation.AddAsync(d).AsTask();
    public Task AddValidationAsync(Validation v) => _db.Validations.AddAsync(v).AsTask();
    public Task SaveAsync() => _db.SaveChangesAsync();
}
