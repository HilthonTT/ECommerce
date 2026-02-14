namespace ECommerce.Webhooks.Infrastructure.Webhooks;

internal sealed class WebhookPayload
{
    public required Guid Id { get; set; }

    public required string EventType { get; set; } = string.Empty;

    public required Guid SubscriptionId { get; set; }

    public required DateTime Timestamp { get; set; }

    public required object Data { get; set; } = null!;
}
