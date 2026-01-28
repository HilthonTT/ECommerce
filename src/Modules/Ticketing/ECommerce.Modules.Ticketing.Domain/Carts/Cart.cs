namespace ECommerce.Modules.Ticketing.Domain.Carts;

public sealed class Cart
{
    public Guid CustomerId { get; private set; }

    public List<CartItem> Items { get; set; } = [];

    public static Cart CreateDefault(Guid customerId) => new() { CustomerId = customerId };
}
