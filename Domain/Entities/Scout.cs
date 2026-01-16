namespace MangoTaikaDistrict.Domain.Entities;

public class Scout
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? Matricule { get; set; }
    public string Nom { get; set; } = default!;
    public string Prenoms { get; set; } = default!;

    public string? Sexe { get; set; }
    public DateOnly? DateNaissance { get; set; }
    public string? LieuNaissance { get; set; }
    public string? Adresse { get; set; }
    public decimal? GpsLat { get; set; }
    public decimal? GpsLng { get; set; }

    public Guid GroupeId { get; set; }
    public Groupe Groupe { get; set; } = default!;

    public Guid? UniteId { get; set; }
    public Unite? Unite { get; set; }

    public string Statut { get; set; } = "Actif";

    // Lien avec l'utilisateur (pour les scouts qui ont un compte)
    public Guid? UtilisateurId { get; set; }
    public Utilisateur? Utilisateur { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<CarteScout> Cartes { get; set; } = new();
    public List<Assurance> Assurances { get; set; } = new();
    public List<Cotisation> Cotisations { get; set; } = new();
    public List<Nomination> Nominations { get; set; } = new();
    public List<ScoutCompetence> ScoutCompetences { get; set; } = new();
}
