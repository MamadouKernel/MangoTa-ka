namespace MangoTaikaDistrict.Domain.Entities;

public class TicketMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = default!;

    public Guid AuteurId { get; set; }
    public Utilisateur Auteur { get; set; } = default!;

    public string Message { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
