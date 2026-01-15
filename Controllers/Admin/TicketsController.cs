using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Infrastructure.Repositories;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using System.Security.Claims;

namespace MangoTaikaDistrict.Controllers.Admin;

[Authorize]
[Area("Admin")]
public class TicketsController : Controller
{
    private readonly ITicketRepository _repo;
    public TicketsController(ITicketRepository repo) => _repo = repo;

    public async Task<IActionResult> Index() => View(await _repo.GetAllAsync());

    [HttpGet]
    public IActionResult Create() => View(new Ticket { Type = TypeTicket.DEMANDE });

    [HttpPost]
    public async Task<IActionResult> Create(Ticket t)
    {
        if (string.IsNullOrWhiteSpace(t.Sujet) || string.IsNullOrWhiteSpace(t.Description))
        {
            ModelState.AddModelError("", "Sujet et description obligatoires.");
            return View(t);
        }

        t.CreatedById = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        t.CreatedAt = DateTime.UtcNow;
        t.UpdatedAt = DateTime.UtcNow;

        await _repo.AddAsync(t);
        await _repo.SaveAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var t = await _repo.GetByIdAsync(id);
        if (t is null) return NotFound();
        return View(t);
    }
}
