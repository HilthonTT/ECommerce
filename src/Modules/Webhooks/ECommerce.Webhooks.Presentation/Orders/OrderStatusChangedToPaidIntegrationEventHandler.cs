using ECommerce.Common.Application.EventBus;
using ECommerce.Modules.Ticketing.IntegrationEvents;
using ECommerce.Webhooks.Application.Abstractions.Webhooks;

namespace ECommerce.Webhooks.Presentation.Orders;

internal sealed class OrderStatusChangedToPaidIntegrationEventHandler(IWebhookDispatcher webhookDispatcher)
    : IntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
{
    public override async Task Handle(
        OrderStatusChangedToPaidIntegrationEvent integrationEvent, 
        CancellationToken cancellationToken = default)
    {
        await webhookDispatcher.DispatchAsync(
            OrderEventTypes.OrderStatusChangedToPaid,
            integrationEvent, 
            cancellationToken);
    }
}
