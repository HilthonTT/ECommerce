using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Ticketing.Domain.Orders;
using ECommerce.Modules.Ticketing.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace ECommerce.Modules.Ticketing.Application.Orders.SetPaidOrderStatus;

internal sealed class OrderStatusChangedToPaidDomainEventHandler(
    IEventBus eventBus, 
    ILogger<OrderStatusChangedToPaidDomainEventHandler> logger)
    : DomainEventHandler<OrderStatusChangedToPaidDomainEvent>
{
    public override async Task Handle(
        OrderStatusChangedToPaidDomainEvent domainEvent, 
        CancellationToken cancellationToken = default)
    {
        OrderingApiTrace.LogOrderStatusUpdated(logger, domainEvent.OrderId, OrderStatus.Paid);

        var orderStockList = domainEvent.OrderItems.Select(oi => new OrderStockItem
        {
            ProductId = oi.ProductId,
            Units = oi.Units
        }).ToList();

        var orderStatusChangedToPaidIntegrationEvent = new OrderStatusChangedToPaidIntegrationEvent(
            domainEvent.Id, 
            domainEvent.OccurredAtUtc,
            domainEvent.OrderId,
            domainEvent.CustomerId,
            orderStockList);

        await eventBus.PublishAsync(orderStatusChangedToPaidIntegrationEvent, cancellationToken);
    }
}
