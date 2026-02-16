using FluentValidation;

namespace ECommerce.Webhooks.Application.Webhooks.RemoveSubscription;

internal sealed class RemoveSubscriptionCommandValidator : AbstractValidator<RemoveSubscriptionCommand>
{
    public RemoveSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty()
            .WithMessage("Subscription ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}
