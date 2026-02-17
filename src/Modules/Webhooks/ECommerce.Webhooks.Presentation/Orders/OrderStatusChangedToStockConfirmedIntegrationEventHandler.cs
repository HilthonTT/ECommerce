using ECommerce.Common.Application.EventBus;
using ECommerce.Modules.Ticketing.IntegrationEvents;
using ECommerce.Webhooks.Application.Abstractions.Webhooks;

namespace ECommerce.Webhooks.Presentation.Orders;

internal sealed class OrderStatusChangedToStockConfirmedIntegrationEventHandler(IWebhookDispatcher webhookDispatcher)
    : IntegrationEventHandler<OrderStatusChangedToStockConfirmedIntegrationEvent>
{
    public override async Task Handle(
        OrderStatusChangedToStockConfirmedIntegrationEvent integrationEvent, 
        CancellationToken cancellationToken = default)
    {
        await webhookDispatcher.DispatchAsync(
            OrderEventTypes.OrderStatusChangedToStockConfirmed,
            integrationEvent,
            cancellationToken);
    }
}
