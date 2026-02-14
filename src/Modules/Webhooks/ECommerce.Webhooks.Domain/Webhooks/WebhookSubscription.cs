namespace ECommerce.Webhooks.Domain.Webhooks;

public sealed record WebhookSubscription(Guid Id, Guid UserId, string EventType, string WebhookUrl, DateTime CreatedAtUtc);
