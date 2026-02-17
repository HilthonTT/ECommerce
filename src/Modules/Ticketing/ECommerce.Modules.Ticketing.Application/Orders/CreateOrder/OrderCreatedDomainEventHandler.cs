using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Ticketing.Domain.Orders;
using ECommerce.Modules.Ticketing.IntegrationEvents;

namespace ECommerce.Modules.Ticketing.Application.Orders.CreateOrder;

internal sealed class OrderCreatedDomainEventHandler(IEventBus eventBus) 
    : DomainEventHandler<OrderCreatedDomainEvent>
{
    public override async Task Handle(OrderCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var @event = new OrderCreatedIntegrationEvent(
            domainEvent.Id,
            domainEvent.OccurredAtUtc, 
            domainEvent.OrderId, 
            domainEvent.CustomerId);

        await eventBus.PublishAsync(@event, cancellationToken);
    }
}
