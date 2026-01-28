using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Orders;

public sealed class OrderCancelledDomainEvent(Guid orderId) : DomainEvent
{
    public Guid OrderId { get; } = orderId;
}