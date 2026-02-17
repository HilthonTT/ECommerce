using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Orders;

public sealed class OrderStatusChangedToPaidDomainEvent(Guid orderId, Guid customerId, List<OrderItem> orderItems) : DomainEvent
{
    public Guid OrderId { get; } = orderId;

    public Guid CustomerId { get; } = customerId;

    public List<OrderItem> OrderItems { get; } = orderItems;
}
