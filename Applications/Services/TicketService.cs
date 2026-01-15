using MangoTaikaDistrict.Domain.Entities;
using MangoTaikaDistrict.Domain.Enums;
using MangoTaikaDistrict.Infrastructure.Repositories;

namespace MangoTaikaDistrict.Application.Services;

public class TicketService
{
    private readonly ITicketRepository _repo;
    public TicketService(ITicketRepository repo) => _repo = repo;

    public async Task CreateAsync(Guid userId, string sujet, string description, TypeTicket type)
    {
        var t = new Ticket
        {
            CreatedById = userId,
            Sujet = sujet,
            Description = description,
            Type = type,
            Priorite = PrioriteTicket.MOYENNE,
            Statut = StatutTicket.OUVERT
        };
        await _repo.AddAsync(t);
        await _repo.SaveAsync();
    }
}
