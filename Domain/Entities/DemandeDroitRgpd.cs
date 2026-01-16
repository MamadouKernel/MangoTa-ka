using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Domain.Entities;

/// <summary>
/// Demandes de droits RGPD (accès, rectification, suppression, opposition)
/// </summary>
public class DemandeDroitRgpd
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; } = default!;
    
    public TypeDemandeRgpd Type { get; set; }
    public string? Description { get; set; }
    public string? Raison { get; set; }
    
    public StatutDemandeRgpd Statut { get; set; } = StatutDemandeRgpd.EN_ATTENTE;
    
    public Guid? TraiteParId { get; set; }
    public Utilisateur? TraitePar { get; set; }
    public DateTime? TraiteLe { get; set; }
    public string? Reponse { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
