using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Tickets;

public sealed class TicketClosedDomainEvent(Guid ticketId, string code) : DomainEvent
{
    public Guid TicketId { get; } = ticketId;

    public string Code { get; } = code;
}