using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public TypeTicket Type { get; set; } = TypeTicket.DEMANDE;
    public PrioriteTicket Priorite { get; set; } = PrioriteTicket.MOYENNE;
    public StatutTicket Statut { get; set; } = StatutTicket.OUVERT;

    public string Sujet { get; set; } = default!;
    public string Description { get; set; } = default!;

    public Guid CreatedById { get; set; }
    public Utilisateur CreatedBy { get; set; } = default!;

    public Guid? AssignedToId { get; set; }
    public Utilisateur? AssignedTo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<TicketMessage> Messages { get; set; } = new();
}
