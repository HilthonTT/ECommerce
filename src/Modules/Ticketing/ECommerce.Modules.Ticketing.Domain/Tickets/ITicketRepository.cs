namespace ECommerce.Modules.Ticketing.Domain.Tickets;

public interface ITicketRepository
{
    Task<Ticket?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Ticket?> GetByCode(string code, CancellationToken cancellationToken = default);

    Task<Ticket?> GetWithoutIncludeAsync(Guid id, CancellationToken cancellationToken = default);

    void Insert(Ticket ticket);

    void InsertRange(IEnumerable<Ticket> tickets);

    void Remove(Ticket ticket);
}
