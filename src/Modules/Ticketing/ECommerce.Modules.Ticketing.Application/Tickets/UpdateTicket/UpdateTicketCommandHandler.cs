using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Tickets;

namespace ECommerce.Modules.Ticketing.Application.Tickets.UpdateTicket;

internal sealed class UpdateTicketCommandHandler(
    ITicketRepository ticketRepository, 
    IUnitOfWork unitOfWork) 
    : ICommandHandler<UpdateTicketCommand>
{
    public async Task<Result> Handle(UpdateTicketCommand command, CancellationToken cancellationToken)
    {
        Ticket? ticket = await ticketRepository.GetWithoutIncludeAsync(command.TicketId, cancellationToken);
        if (ticket is null)
        {
            return TicketErrors.NotFound(command.TicketId);
        }

        ticket.UpdateInfo(command.ProductId, command.Type, command.Status);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
