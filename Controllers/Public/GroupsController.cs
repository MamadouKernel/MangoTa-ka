using Microsoft.AspNetCore.Mvc;
using MangoTaikaDistrict.Infrastructure.Repositories;

namespace MangoTaikaDistrict.Controllers.Public;

public class GroupsController : Controller
{
    private readonly IGroupeRepository _repo;
    public GroupsController(IGroupeRepository repo) => _repo = repo;

    public async Task<IActionResult> Index()
        => View(await _repo.GetAllAsync());
}
