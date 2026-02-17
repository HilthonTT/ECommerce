using ECommerce.Common.Application.EventBus;

namespace ECommerce.Modules.Ticketing.IntegrationEvents;

public sealed class OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent
{
    public OrderStatusChangedToPaidIntegrationEvent(
        Guid id, 
        DateTime occurredAtUtc, 
        Guid orderId, 
        Guid userId,
        List<OrderStockItem> orderStockItems)
        : base(id, occurredAtUtc)
    {
        OrderId = orderId;
        UserId = userId;
        OrderStockItems = orderStockItems;
    }

    public Guid OrderId { get; init; }

    public Guid UserId { get; init; }

    public List<OrderStockItem> OrderStockItems { get; set; } = [];
}
