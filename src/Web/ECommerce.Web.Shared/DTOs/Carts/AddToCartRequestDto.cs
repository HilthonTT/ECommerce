namespace ECommerce.Web.Shared.DTOs.Carts;

public sealed record AddToCartRequestDto
{
    public required int ProductId { get; init; }

    public required int Quantity { get; init; }
}
