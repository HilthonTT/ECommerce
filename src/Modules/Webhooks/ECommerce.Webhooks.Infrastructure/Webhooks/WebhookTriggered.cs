namespace ECommerce.Webhooks.Infrastructure.Webhooks;

internal record WebhookTriggered(Guid SubscriptionId, string EventType, string WebhookUrl, object Data);
