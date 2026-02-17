using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Ticketing.Domain.Orders;
using ECommerce.Modules.Ticketing.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace ECommerce.Modules.Ticketing.Application.Orders.SetAwaitingOrderStatus;

internal sealed class OrderStatusChangedToAwaitingDomainEventHandler(
    IEventBus eventBus, 
    ILogger<OrderStatusChangedToAwaitingDomainEventHandler> logger)
    : DomainEventHandler<OrderStatusChangedToAwaitingDomainEvent>
{
    public override async Task Handle(
        OrderStatusChangedToAwaitingDomainEvent domainEvent, 
        CancellationToken cancellationToken = default)
    {
        OrderingApiTrace.LogOrderStatusUpdated(logger, domainEvent.OrderId, OrderStatus.AwaitingValidation);

        var orderStockList = domainEvent.OrderItems.Select(oi => new OrderStockItem
        {
            ProductId = oi.ProductId,
            Units = oi.Units
        }).ToList();

        var integrationEvent = new OrderStatusChangedToAwaitingIntegrationEvent(
            domainEvent.OrderId, 
            domainEvent.OccurredAtUtc,
            domainEvent.OrderId,
            domainEvent.CustomerId,
            orderStockList);

        await eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
