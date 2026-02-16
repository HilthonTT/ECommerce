using FluentValidation;

namespace ECommerce.Webhooks.Application.Webhooks.CreateSubscription;

internal sealed class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.EventType)
            .NotEmpty()
            .WithMessage("Event type is required.")
            .MaximumLength(100)
            .WithMessage("Event type must not exceed 100 characters.")
            .Matches("^[a-zA-Z0-9._-]+$")
            .WithMessage("Event type can only contain alphanumeric characters, dots, underscores, and hyphens.");

        RuleFor(x => x.WebhookUrl)
            .NotEmpty()
            .WithMessage("Webhook URL is required.")
            .MaximumLength(2000)
            .WithMessage("Webhook URL must not exceed 2000 characters.")
            .Must(BeAValidUrl)
            .WithMessage("Webhook URL must be a valid HTTP or HTTPS URL.");
    }

    private static bool BeAValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
        {
            return false;
        }

        return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
    }
}
