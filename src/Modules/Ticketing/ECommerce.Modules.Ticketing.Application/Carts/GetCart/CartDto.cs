namespace ECommerce.Modules.Ticketing.Application.Carts.GetCart;

public sealed class CartDto
{
    public required Guid CustomerId { get; set; }

    public List<CartItemDto> Items { get; set; } = [];
}
