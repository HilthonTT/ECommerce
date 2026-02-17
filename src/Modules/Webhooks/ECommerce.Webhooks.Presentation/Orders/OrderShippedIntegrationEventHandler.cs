using ECommerce.Common.Application.EventBus;
using ECommerce.Modules.Ticketing.IntegrationEvents;
using ECommerce.Webhooks.Application.Abstractions.Webhooks;

namespace ECommerce.Webhooks.Presentation.Orders;

internal sealed class OrderShippedIntegrationEventHandler(IWebhookDispatcher webhookDispatcher) 
    : IntegrationEventHandler<OrderShippedIntegrationEvent>
{
    public override async Task Handle(OrderShippedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        await webhookDispatcher.DispatchAsync(
            OrderEventTypes.OrderStatusChangedToShipped,
            integrationEvent,
            cancellationToken);
    }
}
