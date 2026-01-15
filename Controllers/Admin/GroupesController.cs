using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Repositories;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize(Roles = "ADMIN,GESTIONNAIRE,SUPERVISEUR")]
[Area("Admin")]
public class GroupesController : Controller
{
    private readonly IGroupeRepository _repo;
    public GroupesController(IGroupeRepository repo) => _repo = repo;

    public async Task<IActionResult> Index()
        => View(await _repo.GetAllAsync());

    [HttpGet]
    public IActionResult Create() => View(new Groupe());

    [HttpPost]
    public async Task<IActionResult> Create(Groupe g)
    {
        if (!ModelState.IsValid) return View(g);
        await _repo.AddAsync(g);
        await _repo.SaveAsync();
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

        db.Nom = g.Nom;
        db.Adresse = g.Adresse;
        db.ContactTel = g.ContactTel;
        db.ContactEmail = g.ContactEmail;
        db.GpsLat = g.GpsLat;
        db.GpsLng = g.GpsLng;
        db.IsActive = g.IsActive;
        db.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveAsync();
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

        await _repo.DeleteAsync(g);
        await _repo.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}
