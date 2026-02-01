using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Tickets.CreateTicket;

public sealed record CreateTicketCommand(Guid CustomerId, Guid? ProductId, string Message) : ICommand;
