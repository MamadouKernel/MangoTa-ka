using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Infrastructure.Data;
using MangoTaikaDistrict.Domain.Entities;

namespace MangoTaikaDistrict.Controllers.Public;

public class HomeController : Controller
{
    private readonly IMotCommissaireRepository _motRepo;
    private readonly IContentRepository _contentRepo;
    private readonly IGroupeRepository _groupeRepo;
    private readonly IScoutRepository _scoutRepo;
    private readonly AppDbContext _db;

    public HomeController(
        IMotCommissaireRepository motRepo, 
        IContentRepository contentRepo,
        IGroupeRepository groupeRepo,
        IScoutRepository scoutRepo,
        AppDbContext db)
    {
        _motRepo = motRepo;
        _contentRepo = contentRepo;
        _groupeRepo = groupeRepo;
        _scoutRepo = scoutRepo;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        // Mot du commissaire
        ViewBag.MotCommissaire = await _motRepo.GetActiveAsync();
        
        // Actualités récentes (3 dernières)
        var allNews = await _contentRepo.GetPublishedNewsAsync();
        ViewBag.RecentNews = allNews.Take(3).ToList();
        ViewBag.Banniere = allNews.FirstOrDefault();
        
        // Statistiques
        var totalGroupes = await _groupeRepo.GetAllAsync();
        var groupesActifs = totalGroupes.Where(g => g.IsActive).ToList();
        ViewBag.TotalGroupes = groupesActifs.Count;
        
        var totalScouts = await _scoutRepo.GetAllAsync();
        var scoutsActifs = totalScouts.Where(s => s.Statut == "Actif").ToList();
        ViewBag.TotalScouts = scoutsActifs.Count;
        
        // Actualités publiées
        ViewBag.TotalActualites = allNews.Count;
        
        // Albums publics (3 derniers)
        var albums = await _contentRepo.GetPublicAlbumsAsync();
        ViewBag.RecentAlbums = albums.Take(3).ToList();
        
        // Messages du livre d'or (3 derniers)
        var livreOr = await _contentRepo.GetGuestbookPublicAsync();
        ViewBag.RecentMessages = livreOr.Take(3).ToList();
        
        // Formations actives
        var formations = await _db.Formations
            .Where(f => f.EstActive && f.EstPublique)
            .OrderByDescending(f => f.CreatedAt)
            .Take(3)
            .ToListAsync();
        ViewBag.RecentFormations = formations;
        
        return View();
    }
}
