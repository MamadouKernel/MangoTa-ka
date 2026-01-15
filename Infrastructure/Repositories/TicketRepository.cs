using Microsoft.EntityFrameworkCore;
using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Infrastructure.Data;

namespace MangoTaikaDistrict.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _db;
    public TicketRepository(AppDbContext db) => _db = db;

    public Task<List<Ticket>> GetAllAsync() =>
        _db.Tickets
          .Include(t => t.CreatedBy)
          .Include(t => t.AssignedTo)
          .OrderByDescending(t => t.CreatedAt)
          .ToListAsync();

    public Task<Ticket?> GetByIdAsync(Guid id) =>
        _db.Tickets
          .Include(t => t.CreatedBy)
          .Include(t => t.AssignedTo)
          .Include(t => t.Messages).ThenInclude(m => m.Auteur)
          .FirstOrDefaultAsync(t => t.Id == id);

    public Task AddAsync(Ticket t) => _db.Tickets.AddAsync(t).AsTask();
    public Task SaveAsync() => _db.SaveChangesAsync();
}
