namespace ECommerce.Webhooks.Presentation.Orders;

internal static class OrderEventTypes
{
    internal const string OrderCreated = "order.created";
    internal const string OrderStatusChangedToPaid = "order.status_changed_to_paid";
    internal const string OrderStatusChangedToAwaiting = "order.status_changed_to_awaiting";
    internal const string OrderStatusChangedToStockConfirmed = "order.status_changed_to_stock_confirmed";
    internal const string OrderStatusChangedToCancelled = "order.status_changed_to_cancelled";
    internal const string OrderStatusChangedToShipped = "order.status_changed_to_shipped";
}
