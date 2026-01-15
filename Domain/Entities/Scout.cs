namespace MangoTaikaDistrict.Domain.Entities;

public class Scout
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? Matricule { get; set; }
    public string Nom { get; set; } = default!;
    public string Prenoms { get; set; } = default!;

    public string? Sexe { get; set; }
    public DateOnly? DateNaissance { get; set; }

    public Guid GroupeId { get; set; }
    public Groupe Groupe { get; set; } = default!;

    public Guid? UniteId { get; set; }
    public Unite? Unite { get; set; }

    public string Statut { get; set; } = "Actif";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<CarteScout> Cartes { get; set; } = new();
    public List<Assurance> Assurances { get; set; } = new();
}
