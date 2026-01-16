using System.ComponentModel;
using System.Diagnostics;

namespace MangoTaikaDistrict.Domain.Entities;

public class Groupe
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nom { get; set; } = default!;
    public string? Adresse { get; set; }
    public string? ContactTel { get; set; }
    public string? ContactEmail { get; set; }

    public decimal? GpsLat { get; set; }
    public decimal? GpsLng { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<Unite> Unites { get; set; } = new();
    public List<Scout> Scouts { get; set; } = new();
    public List<Activite> Activites { get; set; } = new();
    public List<Cotisation> Cotisations { get; set; } = new();
    public List<Nomination> Nominations { get; set; } = new();
}
