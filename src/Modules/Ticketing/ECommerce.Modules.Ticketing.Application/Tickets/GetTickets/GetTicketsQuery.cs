using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Ticketing.Domain.Tickets;

namespace ECommerce.Modules.Ticketing.Application.Tickets.GetTickets;

public sealed record GetTicketsQuery(
    Guid CustomerId, 
    List<Guid> BrandIds, 
    TicketStatus? Status, 
    string? Sort,
    int StartIndex,
    int MaxResults) 
    : IQuery<GetTicketsResponse>;
