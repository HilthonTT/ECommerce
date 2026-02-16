using ECommerce.Common.Domain;

namespace ECommerce.Webhooks.Domain.Webhooks;

public static class WebhookErrors
{
    public static readonly Error AlreadyExists = Error.Conflict(
        "Webhook.AlreadyExists",
        "The webhook subscription already exists");

    public static Error NotFound(Guid subscriptionId) => Error.NotFound(
        "Webhook.NotFound",
        $"The webhook subscription with id {subscriptionId} was not found");
}
