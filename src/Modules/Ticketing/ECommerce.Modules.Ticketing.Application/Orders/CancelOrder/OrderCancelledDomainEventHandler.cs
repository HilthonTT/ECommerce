using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Ticketing.Domain.Orders;
using ECommerce.Modules.Ticketing.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace ECommerce.Modules.Ticketing.Application.Orders.CancelOrder;

internal sealed class OrderCancelledDomainEventHandler(
    IEventBus eventBus,
    ILogger<OrderCancelledDomainEventHandler> logger) : DomainEventHandler<OrderCancelledDomainEvent>
{
    public override async Task Handle(OrderCancelledDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        OrderingApiTrace.LogOrderStatusUpdated(logger, domainEvent.OrderId, OrderStatus.Cancelled);

        var @event = new OrderCancelledIntegrationEvent(
            domainEvent.Id,
            domainEvent.OccurredAtUtc, 
            domainEvent.OrderId, 
            domainEvent.CustomerId);

        await eventBus.PublishAsync(@event, cancellationToken);
    }
}
