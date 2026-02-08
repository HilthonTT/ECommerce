namespace ECommerce.Webhooks.Domain.Webhooks;

public sealed record WebhookSubscription(Guid Id, string EventType, string WebhookUrl, DateTime CreatedAtUtc);
