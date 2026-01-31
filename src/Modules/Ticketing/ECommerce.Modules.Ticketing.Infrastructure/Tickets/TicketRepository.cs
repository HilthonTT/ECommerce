using ECommerce.Modules.Ticketing.Domain.Tickets;
using ECommerce.Modules.Ticketing.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Ticketing.Infrastructure.Tickets;

internal sealed class TicketRepository(TicketingDbContext dbContext) : ITicketRepository
{
    public Task<Ticket?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Tickets
            .Include(t => t.Product)
            .Include(t => t.Customer)
            .Include(t => t.Messages)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public Task<Ticket?> GetByCode(string code, CancellationToken cancellationToken = default)
    {
        return dbContext.Tickets
             .Include(t => t.Product)
             .Include(t => t.Customer)
             .Include(t => t.Messages)
             .AsSplitQuery()
             .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);
    }

    public void Insert(Ticket ticket)
    {
        dbContext.Tickets.Add(ticket);
    }

    public void InsertRange(IEnumerable<Ticket> tickets)
    {
        dbContext.Tickets.AddRange(tickets);
    }

    public void Remove(Ticket ticket)
    {
        dbContext.Tickets.Remove(ticket);
    }
}
