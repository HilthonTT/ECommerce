using ECommerce.Common.Application.EventBus;

namespace ECommerce.Modules.Ticketing.IntegrationEvents;

public sealed class OrderCreatedIntegrationEvent : IntegrationEvent
{
    public OrderCreatedIntegrationEvent(Guid id, DateTime occurredAtUtc, Guid orderId, Guid userId)
        : base(id, occurredAtUtc)
    {
        OrderId = orderId;
        UserId = userId;
    }

    public Guid OrderId { get; init; }

    public Guid UserId { get; init; }
}
