using ECommerce.Modules.Ticketing.Domain.Messages;
using ECommerce.Modules.Ticketing.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Ticketing.Infrastructure.Messages;

internal sealed class MessageRepository(TicketingDbContext dbContext) : IMessageRepository
{
    public Task<Message?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Messages
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public Task<List<Message>> GetByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return dbContext.Messages
            .Where(m => m.TicketId == ticketId)
            .ToListAsync(cancellationToken);
    }

    public void Insert(Message message)
    {
        dbContext.Messages.Add(message);
    }
}
