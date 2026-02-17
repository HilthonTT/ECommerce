using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Ticketing.Domain.Orders;
using ECommerce.Modules.Ticketing.IntegrationEvents;

namespace ECommerce.Modules.Ticketing.Application.Orders.ShipOrder;

internal sealed class OrderShippedDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<OrderShippedDomainEvent>
{
    public override async Task Handle(OrderShippedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var integrationEvent = new OrderShippedIntegrationEvent(
            domainEvent.Id,
            domainEvent.OccurredAtUtc, 
            domainEvent.OrderId, 
            domainEvent.CustomerId);

        await eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
