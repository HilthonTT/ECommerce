using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Orders;

public sealed class OrderShippedDomainEvent(Guid orderId, Guid customerId) : DomainEvent
{
    public Guid OrderId { get; } = orderId;

    public Guid CustomerId { get; } = customerId;
}