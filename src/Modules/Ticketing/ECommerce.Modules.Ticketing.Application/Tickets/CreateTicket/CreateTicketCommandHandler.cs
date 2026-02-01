using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.AI;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Messages;
using ECommerce.Modules.Ticketing.Domain.Tickets;

namespace ECommerce.Modules.Ticketing.Application.Tickets.CreateTicket;

internal sealed class CreateTicketCommandHandler(
    ITicketRepository ticketRepository, 
    IUnitOfWork unitOfWork,
    ITicketSummarizer ticketSummarizer,
    IPythonInferenceClient pythonInferenceClient) : ICommandHandler<CreateTicketCommand>
{
    public async Task<Result> Handle(CreateTicketCommand command, CancellationToken cancellationToken)
    {
        // Classify the new ticket using the small zero-shot classifier model
        TicketType[] ticketTypes = Enum.GetValues<TicketType>();
        string? inferredTicketType = await pythonInferenceClient.ClassifyTextAsync(
            command.Message,
            candidateLabels: ticketTypes.Select(type => type.ToString()),
            cancellationToken);

        TicketType ticketType = Enum.TryParse<TicketType>(inferredTicketType, out var type) ? type : TicketType.Question;

        Result<Ticket> ticketResult = Ticket.Create(
            command.CustomerId,
            ticketType,
            command.ProductId);

        if (ticketResult.IsFailure)
        {
            return ticketResult.Error;
        }

        Ticket ticket = ticketResult.Value;
        ticket.Messages.Add(Message.Create(ticket.Id, command.Message, isCustomerMessage: true));

        ticketRepository.Insert(ticket);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        ticketSummarizer.UpdateSummary(ticket.Id, enforceRateLimit: true);

        return Result.Success();
    }
}
