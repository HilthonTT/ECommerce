namespace ECommerce.Modules.Ticketing.Presentation;

internal static class Permissions
{
    internal const string CreateTicket = "tickets:create";
    internal const string ViewTickets = "tickets:view";
    internal const string UpdateTickets = "tickets:update";
    internal const string ChatAssistant = "tickets:chat-assistant";

    internal const string AddToCart = "carts:add";
    internal const string ViewCart = "carts:view";
    internal const string RemoveFromCart = "carts:remove";

    internal const string CreateOrder = "orders:create";
    internal const string ViewOrders = "orders:view";
    internal const string ShipOrder = "orders:ship";
    internal const string CancelOrder = "orders:cancel";
}
