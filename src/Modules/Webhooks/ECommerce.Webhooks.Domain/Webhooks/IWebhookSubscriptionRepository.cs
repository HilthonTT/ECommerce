namespace ECommerce.Webhooks.Domain.Webhooks;

public interface IWebhookSubscriptionRepository
{
    Task<bool> ExistsAsync(Guid userId, string eventType, string webhookUrl, CancellationToken cancellationToken = default);

    Task<WebhookSubscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Insert(WebhookSubscription subscription);

    void Remove(WebhookSubscription subscription);
}
