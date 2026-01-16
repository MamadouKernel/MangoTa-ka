namespace MangoTaikaDistrict.Domain.Entities;

/// <summary>
/// Formation proposée dans le LMS
/// </summary>
public class Formation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titre { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? Contenu { get; set; } // Contenu HTML ou markdown
    public string? ImageUrl { get; set; }
    public int DureeEstimee { get; set; } // En heures
    public string Niveau { get; set; } = "Débutant"; // Débutant, Intermédiaire, Avancé
    public bool EstActive { get; set; } = true;
    public bool EstPublique { get; set; } = false; // Accessible à tous ou seulement aux inscrits
    public int OrdreAffichage { get; set; } = 0;

    // Relations
    public List<ModuleFormation> Modules { get; set; } = new();
    public List<InscriptionFormation> Inscriptions { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedById { get; set; }
    public Utilisateur? CreatedBy { get; set; }
}
