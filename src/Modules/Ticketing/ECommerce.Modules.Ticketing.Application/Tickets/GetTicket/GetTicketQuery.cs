using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Tickets.GetTicket;

public sealed record GetTicketQuery(Guid TicketId, Guid CustomerId) : IQuery<TicketDetailsResult>;
