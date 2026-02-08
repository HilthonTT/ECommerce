namespace ECommerce.Modules.Ticketing.Application.Carts.GetCart;

public sealed class CartItemDto
{
    public required Guid ProductId { get; set; }
    public required string ProductName { get; set; } = string.Empty;
    public required decimal UnitPrice { get; set; }
    public required decimal OldUnitPrice { get; set; }
    public required int Quantity { get; set; }
    public required string PictureUrl { get; set; } = string.Empty;
    public required string Currency { get; set; } = string.Empty;
}