using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Infrastructure.Data;
using MangoTaikaDistrict.Domain.Enums;

namespace MangoTaikaDistrict.Controllers.Public;

public class GroupsController : Controller
{
    private readonly IGroupeRepository _repo;
    private readonly AppDbContext _db;

    public GroupsController(IGroupeRepository repo, AppDbContext db)
    {
        _repo = repo;
        _db = db;
    }

    public async Task<IActionResult> Index()
        => View(await _repo.GetAllAsync());

    [HttpGet]
    public async Task<IActionResult> Map()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> MapData()
    {
        var groupes = await _db.Groupes
            .Include(g => g.Nominations)
                .ThenInclude(n => n.Scout)
            .Where(g => g.GpsLat.HasValue && g.GpsLng.HasValue && g.IsActive)
            .ToListAsync();

        var data = groupes.Select(g => new
        {
            id = g.Id,
            nom = g.Nom,
            adresse = g.Adresse,
            lat = g.GpsLat,
            lng = g.GpsLng,
            contactTel = g.ContactTel,
            contactEmail = g.ContactEmail,
            maitrise = new
            {
                cg = g.Nominations
                    .Where(n => n.Poste.Contains("Commissaire de Groupe", StringComparison.OrdinalIgnoreCase) 
                        && n.Statut == StatutNomination.VALIDEE
                        && (n.DateFin == null || n.DateFin >= DateOnly.FromDateTime(DateTime.Now)))
                    .Select(n => $"{n.Scout.Nom} {n.Scout.Prenoms}")
                    .FirstOrDefault(),
                adjoints = g.Nominations
                    .Where(n => n.Poste.Contains("Adjoint", StringComparison.OrdinalIgnoreCase) 
                        && n.Statut == StatutNomination.VALIDEE
                        && (n.DateFin == null || n.DateFin >= DateOnly.FromDateTime(DateTime.Now)))
                    .Select(n => $"{n.Scout.Nom} {n.Scout.Prenoms}")
                    .ToList(),
                oisillons = g.Nominations
                    .Where(n => (n.Poste.Contains("Oisillons", StringComparison.OrdinalIgnoreCase) 
                        || n.Poste.Contains("Oisillon", StringComparison.OrdinalIgnoreCase))
                        && n.Statut == StatutNomination.VALIDEE
                        && (n.DateFin == null || n.DateFin >= DateOnly.FromDateTime(DateTime.Now)))
                    .Select(n => $"{n.Scout.Nom} {n.Scout.Prenoms}")
                    .FirstOrDefault(),
                louveteaux = g.Nominations
                    .Where(n => (n.Poste.Contains("Louveteaux", StringComparison.OrdinalIgnoreCase) 
                        || n.Poste.Contains("Louveteau", StringComparison.OrdinalIgnoreCase))
                        && n.Statut == StatutNomination.VALIDEE
                        && (n.DateFin == null || n.DateFin >= DateOnly.FromDateTime(DateTime.Now)))
                    .Select(n => $"{n.Scout.Nom} {n.Scout.Prenoms}")
                    .FirstOrDefault(),
                eclaireurs = g.Nominations
                    .Where(n => (n.Poste.Contains("Eclaireurs", StringComparison.OrdinalIgnoreCase) 
                        || n.Poste.Contains("Éclaireurs", StringComparison.OrdinalIgnoreCase)
                        || n.Poste.Contains("Eclaireur", StringComparison.OrdinalIgnoreCase))
                        && n.Statut == StatutNomination.VALIDEE
                        && (n.DateFin == null || n.DateFin >= DateOnly.FromDateTime(DateTime.Now)))
                    .Select(n => $"{n.Scout.Nom} {n.Scout.Prenoms}")
                    .FirstOrDefault(),
                cheminots = g.Nominations
                    .Where(n => (n.Poste.Contains("Cheminots", StringComparison.OrdinalIgnoreCase) 
                        || n.Poste.Contains("Cheminot", StringComparison.OrdinalIgnoreCase))
                        && n.Statut == StatutNomination.VALIDEE
                        && (n.DateFin == null || n.DateFin >= DateOnly.FromDateTime(DateTime.Now)))
                    .Select(n => $"{n.Scout.Nom} {n.Scout.Prenoms}")
                    .FirstOrDefault(),
                routiers = g.Nominations
                    .Where(n => (n.Poste.Contains("Routiers", StringComparison.OrdinalIgnoreCase) 
                        || n.Poste.Contains("Routier", StringComparison.OrdinalIgnoreCase))
                        && n.Statut == StatutNomination.VALIDEE
                        && (n.DateFin == null || n.DateFin >= DateOnly.FromDateTime(DateTime.Now)))
                    .Select(n => $"{n.Scout.Nom} {n.Scout.Prenoms}")
                    .FirstOrDefault()
            }
        }).ToList();

        return Json(data);
    }
}
