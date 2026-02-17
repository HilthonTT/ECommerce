using ECommerce.Common.Application.EventBus;
using ECommerce.Modules.Ticketing.IntegrationEvents;
using ECommerce.Webhooks.Application.Abstractions.Webhooks;

namespace ECommerce.Webhooks.Presentation.Orders;

internal sealed class OrderCreatedIntegrationEventHandler(IWebhookDispatcher webhookDispatcher)
    : IntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    public override async Task Handle(
        OrderCreatedIntegrationEvent integrationEvent, 
        CancellationToken cancellationToken = default)
    {
        await webhookDispatcher.DispatchAsync(
            OrderEventTypes.OrderCreated,
            integrationEvent,
            cancellationToken);
    }
}
