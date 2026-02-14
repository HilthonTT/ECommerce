using ECommerce.Webhooks.Domain.Webhooks;
using ECommerce.Webhooks.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Webhooks.Infrastructure.Webhooks;

internal sealed class WebhookSubcriptionRepository(WebhooksDbContext webhooksDbContext) : IWebhookSubscriptionRepository
{
    public Task<WebhookSubscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return webhooksDbContext.WebhookSubscriptions.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public void Insert(WebhookSubscription subscription)
    {
        webhooksDbContext.WebhookSubscriptions.Add(subscription);
    }

    public void Remove(WebhookSubscription subscription)
    {
        webhooksDbContext.WebhookSubscriptions.Remove(subscription);
    }
}
