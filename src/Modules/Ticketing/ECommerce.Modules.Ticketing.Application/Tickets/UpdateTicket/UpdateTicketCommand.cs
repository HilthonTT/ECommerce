using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Ticketing.Domain.Tickets;

namespace ECommerce.Modules.Ticketing.Application.Tickets.UpdateTicket;

public sealed record UpdateTicketCommand(Guid TicketId, Guid? ProductId, TicketType Type, TicketStatus Status) : ICommand;
