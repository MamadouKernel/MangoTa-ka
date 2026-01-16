using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using MangoTaikaDistrict.Infrastructure.Repositories;

namespace MangoTaikaDistrict.Application.Services;

public class WorkflowService
{
    private readonly IWorkflowRepository _repo;
    public WorkflowService(IWorkflowRepository repo) => _repo = repo;

    public async Task<(bool ok, string? error)> SoumettreAutorisationAsync(Guid activiteId, Guid demandeurId)
    {
        var activite = await _repo.GetActiviteAsync(activiteId);
        if (activite is null) return (false, "Activité introuvable.");

        if (activite.DemandeAutorisation != null) return (false, "Demande déjà créée.");

        activite.Statut = StatutDemande.EN_ATTENTE;

        var demande = new DemandeAutorisation
        {
            ActiviteId = activite.Id,
            DemandeurId = demandeurId,
            Statut = StatutDemande.EN_ATTENTE,
            CurrentStep = 1
        };

        await _repo.AddDemandeAsync(demande);
        await _repo.SaveAsync();
        return (true, null);
    }

    public async Task<(bool ok, string? error)> DeciderAsync(Guid demandeId, Guid valideurId, bool approve, string? commentaire)
    {
        var demande = await _repo.GetDemandeAsync(demandeId);
        if (demande is null) return (false, "Demande introuvable.");

        if (demande.Statut != StatutDemande.EN_ATTENTE)
            return (false, "Cette demande n'est plus en attente.");

        var circuit = await _repo.GetCircuitAsync("AUTORISATION_ACTIVITE");
        if (circuit is null) return (false, "Circuit introuvable/inactif.");

        // Ajout validation
        await _repo.AddValidationAsync(new Validation
        {
            DemandeAutorisationId = demande.Id,
            StepOrder = demande.CurrentStep,
            ValideurId = valideurId,
            Decision = approve ? DecisionValidation.APPROUVE : DecisionValidation.REJETE,
            Commentaire = commentaire
        });

        if (!approve)
        {
            demande.Statut = StatutDemande.REJETEE;
            demande.Activite.Statut = StatutDemande.REJETEE;
            await _repo.SaveAsync();
            return (true, null);
        }

        // approve -> step suivant
        var lastStep = circuit.Etapes.Max(e => e.StepOrder);
        if (demande.CurrentStep >= lastStep)
        {
            demande.Statut = StatutDemande.VALIDEE;
            demande.Activite.Statut = StatutDemande.VALIDEE;
        }
        else
        {
            demande.CurrentStep += 1;
        }

        await _repo.SaveAsync();
        return (true, null);
    }
}
