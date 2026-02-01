using ECommerce.Modules.Ticketing.Domain.Tickets;
using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Tickets.UpdateTicket;

internal sealed class UpdateTicketCommandValidator : AbstractValidator<UpdateTicketCommand>
{
    public UpdateTicketCommandValidator()
    {
        RuleFor(x => x.TicketId)
             .NotEmpty()
             .WithMessage("Ticket ID is required");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .When(x => x.ProductId.HasValue)
            .WithMessage("Product ID must be a valid GUID when specified");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid ticket type")
            .NotEqual(default(TicketType))
            .WithMessage("Ticket type must be specified");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid ticket status")
            .NotEqual(default(TicketStatus))
            .WithMessage("Ticket status must be specified");
    }
}