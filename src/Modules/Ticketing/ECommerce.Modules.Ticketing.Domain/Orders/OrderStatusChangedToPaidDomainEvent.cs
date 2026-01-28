using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Orders;

public sealed class OrderStatusChangedToPaidDomainEvent(Guid orderId, List<OrderItem> orderItems) : DomainEvent
{
    public Guid OrderId { get; } = orderId;

    public List<OrderItem> OrderItems { get; } = orderItems;
}
