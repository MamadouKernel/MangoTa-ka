using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Domain.Entities;

public class LivreOrMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Auteur { get; set; } = default!;
    public string Message { get; set; } = default!;

    public StatutModeration Statut { get; set; } = StatutModeration.EN_ATTENTE;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? ModeratedById { get; set; }
    public Utilisateur? ModeratedBy { get; set; }
    public DateTime? ModeratedAt { get; set; }
}
