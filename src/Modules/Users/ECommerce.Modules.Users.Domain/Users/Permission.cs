namespace ECommerce.Modules.Users.Domain.Users;

public sealed class Permission
{
    // Users
    public static readonly Permission GetUser = new("users:read");
    public static readonly Permission ModifyUser = new("users:update");

    // Carts
    public static readonly Permission GetCart = new("carts:read");
    public static readonly Permission ViewCart = new("carts:view");
    public static readonly Permission AddToCart = new("carts:add");
    public static readonly Permission RemoveFromCart = new("carts:remove");

    // Tickets
    public static readonly Permission CreateTicket = new("tickets:create");
    public static readonly Permission ViewTickets = new("tickets:view");
    public static readonly Permission UpdateTickets = new("tickets:update");
    public static readonly Permission ChatAssistant = new("tickets:chat-assistant");

    // Orders
    public static readonly Permission CreateOrder = new("orders:create");
    public static readonly Permission ViewOrders = new("orders:view");
    public static readonly Permission ShipOrder = new("orders:ship");
    public static readonly Permission CancelOrder = new("orders:cancel");

    // Webhooks
    public static readonly Permission ViewWebhooks = new("webhooks:view");
    public static readonly Permission CreateWebhooks = new("webhooks:create");
    public static readonly Permission UpdateWebhooks = new("webhooks:update");
    public static readonly Permission RemoveWebhooks = new("webhooks:remove");

    public string Code { get; }

    public Permission(string code)
    {
        Code = code;
    }
}
