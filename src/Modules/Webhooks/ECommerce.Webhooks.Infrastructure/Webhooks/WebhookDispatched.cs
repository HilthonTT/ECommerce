namespace ECommerce.Webhooks.Infrastructure.Webhooks;

internal sealed record WebhookDispatched(string EventType, object Data);
