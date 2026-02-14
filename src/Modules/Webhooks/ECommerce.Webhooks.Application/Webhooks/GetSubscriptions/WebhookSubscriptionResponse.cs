namespace ECommerce.Webhooks.Application.Webhooks.GetSubscriptions;

public sealed record WebhookSubscriptionResponse(Guid Id, Guid UserId, string EventType, string WebhookUrl, DateTime CreatedAtUtc);
