using FluentValidation;

namespace ECommerce.Modules.Ticketing.Application.Assistant.GetStreamingChatResponse;

internal sealed class GetStreamChatResponseCommandValidator : AbstractValidator<GetStreamChatResponseCommand>
{
    private const int MaxMessageTextLength = 10000;
    private const int MaxCustomerNameLength = 200;
    private const int MaxTicketSummaryLength = 2000;
    private const int MaxTicketLastCustomerMessageLength = 5000;
    private const int MaxMessagesCount = 50;

    public GetStreamChatResponseCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .MaximumLength(MaxCustomerNameLength)
            .When(x => !string.IsNullOrWhiteSpace(x.CustomerName))
            .WithMessage($"Customer name must not exceed {MaxCustomerNameLength} characters");

        RuleFor(x => x.TicketSummary)
            .MaximumLength(MaxTicketSummaryLength)
            .When(x => !string.IsNullOrWhiteSpace(x.TicketSummary))
            .WithMessage($"Ticket summary must not exceed {MaxTicketSummaryLength} characters");

        RuleFor(x => x.TicketLastCustomerMessage)
            .MaximumLength(MaxTicketLastCustomerMessageLength)
            .When(x => !string.IsNullOrWhiteSpace(x.TicketLastCustomerMessage))
            .WithMessage($"Last customer message must not exceed {MaxTicketLastCustomerMessageLength} characters");

        RuleFor(x => x.Messages)
            .NotNull()
            .WithMessage("Messages collection is required")
            .Must(messages => messages.Count > 0)
            .WithMessage("At least one message is required")
            .Must(messages => messages.Count <= MaxMessagesCount)
            .WithMessage($"Messages count must not exceed {MaxMessagesCount}");

        RuleForEach(x => x.Messages)
            .ChildRules(message =>
            {
                message.RuleFor(m => m.Text)
                    .NotEmpty()
                    .WithMessage("Message text is required")
                    .MaximumLength(MaxMessageTextLength)
                    .WithMessage($"Message text must not exceed {MaxMessageTextLength} characters");
            });
    }
}
