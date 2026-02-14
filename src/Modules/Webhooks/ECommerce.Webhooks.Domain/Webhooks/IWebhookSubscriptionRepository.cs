namespace ECommerce.Webhooks.Domain.Webhooks;

public interface IWebhookSubscriptionRepository
{
    Task<WebhookSubscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Insert(WebhookSubscription subscription);

    void Remove(WebhookSubscription subscription);
}
