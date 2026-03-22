namespace ECommerce.Web.Shared.DTOs.Carts;

public sealed record CartDto
{
    public required Guid CustomerId { get; set; }

    public List<CartItemDto> Items { get; set; } = [];
}
