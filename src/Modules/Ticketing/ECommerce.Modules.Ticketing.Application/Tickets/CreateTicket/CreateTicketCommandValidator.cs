using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Tickets.CreateTicket;

internal sealed class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    private const int MaxMessageLength = 5000;
    private const int MinMessageLength = 1;

    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .When(x => x.ProductId.HasValue)
            .WithMessage("Product ID must be a valid GUID when specified");

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Message is required")
            .MinimumLength(MinMessageLength)
            .WithMessage($"Message must be at least {MinMessageLength} character")
            .MaximumLength(MaxMessageLength)
            .WithMessage($"Message must not exceed {MaxMessageLength} characters");
    }
}