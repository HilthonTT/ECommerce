using ECommerce.Webhooks.Domain.Webhooks;
using ECommerce.Webhooks.Infrastructure.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Webhooks.Infrastructure.Webhooks;

internal sealed class WebhookDispatchedConsumer(WebhooksDbContext dbContext) : IConsumer<WebhookDispatched>
{
    public async Task Consume(ConsumeContext<WebhookDispatched> context)
    {
        WebhookDispatched message = context.Message;

        List<WebhookSubscription> subscriptions = await dbContext.WebhookSubscriptions
            .AsNoTracking()
            .Where(s => s.EventType == message.EventType)
            .ToListAsync(context.CancellationToken);

        IEnumerable<WebhookTriggered> webhookTriggeredEvents = subscriptions.Select(s =>
            new WebhookTriggered(s.Id, message.EventType, s.WebhookUrl, message.Data));

        await context.PublishBatch(webhookTriggeredEvents, context.CancellationToken);
    }
}