using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;
using MangoTaikaDistrict.Infrastructure.Repositories;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize(Roles = "ADMIN,GESTIONNAIRE,SUPERVISEUR")]
[Area("Admin")]
public class GroupesController : Controller
{
    private readonly IGroupeRepository _repo;
    private readonly AppDbContext _db;
    
    public GroupesController(IGroupeRepository repo, AppDbContext db)
    {
        _repo = repo;
        _db = db;
    }

    public async Task<IActionResult> Index()
        => View(await _repo.GetAllAsync());

    [HttpGet]
    public IActionResult Create() => View(new Groupe());

    [HttpPost]
    public async Task<IActionResult> Create(Groupe g)
    {
        if (!ModelState.IsValid) return View(g);
        
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _repo.AddAsync(g);
        await _repo.SaveAsync();
        
        // Historique
        _db.AuditLogs.Add(new AuditLog
        {
            UtilisateurId = userId,
            Action = "CREATE",
            EntityName = "Groupe",
            EntityId = g.Id,
            AfterJson = JsonSerializer.Serialize(g)
        });
        await _db.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var g = await _repo.GetByIdAsync(id);
        if (g is null) return NotFound();
        return View(g);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Groupe g)
    {
        if (!ModelState.IsValid) return View(g);

        var db = await _repo.GetByIdAsync(g.Id);
        if (db is null) return NotFound();

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        // Historique : avant
        var beforeJson = JsonSerializer.Serialize(db);

        db.Nom = g.Nom;
        db.Adresse = g.Adresse;
        db.ContactTel = g.ContactTel;
        db.ContactEmail = g.ContactEmail;
        db.GpsLat = g.GpsLat;
        db.GpsLng = g.GpsLng;
        db.IsActive = g.IsActive;
        db.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveAsync();
        
        // Historique : après
        _db.AuditLogs.Add(new AuditLog
        {
            UtilisateurId = userId,
            Action = "UPDATE",
            EntityName = "Groupe",
            EntityId = g.Id,
            BeforeJson = beforeJson,
            AfterJson = JsonSerializer.Serialize(db)
        });
        await _db.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var g = await _repo.GetByIdAsync(id);
        if (g is null) return NotFound();
        return View(g);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var g = await _repo.GetByIdAsync(id);
        if (g is null) return NotFound();

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var beforeJson = JsonSerializer.Serialize(g);

        await _repo.DeleteAsync(g);
        await _repo.SaveAsync();
        
        // Historique
        _db.AuditLogs.Add(new AuditLog
        {
            UtilisateurId = userId,
            Action = "DELETE",
            EntityName = "Groupe",
            EntityId = id,
            BeforeJson = beforeJson
        });
        await _db.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }
}
