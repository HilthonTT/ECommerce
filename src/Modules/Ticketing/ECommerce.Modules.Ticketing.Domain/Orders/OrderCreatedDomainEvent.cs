using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Orders;

public sealed class OrderCreatedDomainEvent(Guid orderId, Guid customerId) : DomainEvent
{
    public Guid OrderId { get; } = orderId;

    public Guid CustomerId { get; } = customerId;
}