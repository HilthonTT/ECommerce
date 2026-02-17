using ECommerce.Common.Application.EventBus;
using ECommerce.Modules.Ticketing.IntegrationEvents;
using ECommerce.Webhooks.Application.Abstractions.Webhooks;

namespace ECommerce.Webhooks.Presentation.Orders;

internal sealed class OrderStatusChangedToAwaitingIntegrationEventHandler(IWebhookDispatcher webhookDispatcher)
    : IntegrationEventHandler<OrderStatusChangedToAwaitingIntegrationEvent>
{
    public override async Task Handle(
        OrderStatusChangedToAwaitingIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        await webhookDispatcher.DispatchAsync(
            OrderEventTypes.OrderStatusChangedToAwaiting,
            integrationEvent,
            cancellationToken);
    }
}
