using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Ticketing.Domain.Orders;
using ECommerce.Modules.Ticketing.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace ECommerce.Modules.Ticketing.Application.Orders.SetStockConfirmedOrderStatus;

internal sealed class OrderStatusChangedToStockConfirmedDomainEventHandler(
    IEventBus eventBus, 
    ILogger<OrderStatusChangedToStockConfirmedDomainEventHandler> logger)
    : DomainEventHandler<OrderStatusChangedToStockConfirmedDomainEvent>
{
    public override async Task Handle(
        OrderStatusChangedToStockConfirmedDomainEvent domainEvent, 
        CancellationToken cancellationToken = default)
    {
        OrderingApiTrace.LogOrderStatusUpdated(logger, domainEvent.OrderId, OrderStatus.StockConfirmed);

        var integrationEvent = new OrderStatusChangedToStockConfirmedIntegrationEvent(
            domainEvent.Id,
            domainEvent.OccurredAtUtc,
            domainEvent.OrderId,
            domainEvent.CustomerId);

        await eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
