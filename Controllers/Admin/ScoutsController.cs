using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Repositories;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize(Roles = "ADMIN,GESTIONNAIRE")]
[Area("Admin")]
public class ScoutsController : Controller
{
    private readonly IScoutRepository _scouts;
    private readonly IGroupeRepository _groupes;

    public ScoutsController(IScoutRepository scouts, IGroupeRepository groupes)
    {
        _scouts = scouts;
        _groupes = groupes;
    }

    public async Task<IActionResult> Index()
        => View(await _scouts.GetAllAsync());

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Groupes = await _groupes.GetAllAsync();
        return View(new Scout());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Scout s)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Groupes = await _groupes.GetAllAsync();
            return View(s);
        }

        await _scouts.AddAsync(s);
        await _scouts.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var s = await _scouts.GetByIdAsync(id);
        if (s is null) return NotFound();
        ViewBag.Groupes = await _groupes.GetAllAsync();
        return View(s);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Scout s)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Groupes = await _groupes.GetAllAsync();
            return View(s);
        }

        var db = await _scouts.GetByIdAsync(s.Id);
        if (db is null) return NotFound();

        db.Matricule = s.Matricule;
        db.Nom = s.Nom;
        db.Prenoms = s.Prenoms;
        db.Sexe = s.Sexe;
        db.GroupeId = s.GroupeId;
        db.Statut = s.Statut;
        db.UpdatedAt = DateTime.UtcNow;

        await _scouts.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var s = await _scouts.GetByIdAsync(id);
        if (s is null) return NotFound();
        return View(s);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var s = await _scouts.GetByIdAsync(id);
        if (s is null) return NotFound();

        await _scouts.DeleteAsync(s);
        await _scouts.SaveAsync();
        return RedirectToAction(nameof(Index));
    }
}
