using MangoTaikaDistrict.Domain.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace MangoTaikaDistrict.Domain.Entities;

public class DemandeAutorisation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ActiviteId { get; set; }
    public Activite Activite { get; set; } = default!;

    public Guid DemandeurId { get; set; }
    public Utilisateur Demandeur { get; set; } = default!;

    public StatutDemande Statut { get; set; } = StatutDemande.EN_ATTENTE;
    public int CurrentStep { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Validation> Validations { get; set; } = new();
}
