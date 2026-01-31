namespace ECommerce.Modules.Ticketing.Domain.Messages;

public interface IMessageRepository
{
    Task<Message?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<Message>> GetByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default);

    void Insert(Message message);
}
