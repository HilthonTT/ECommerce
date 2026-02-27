using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Ticketing.Domain.Tickets;

namespace ECommerce.Modules.Ticketing.Application.Tickets.UpdateTicket;

public sealed record UpdateTicketCommand(Guid TicketId, int? ProductId, TicketType Type, TicketStatus Status) : ICommand;
