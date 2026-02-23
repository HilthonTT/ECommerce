namespace ECommerce.Modules.Ticketing.Application.Orders;

public sealed class OrderItemDto
{
    public required int ProductId { get; init; }

    public required string ProductName { get; init; }

    public required decimal UnitPrice { get; init; }

    public required decimal Discount { get; init; }

    public required int Units { get; init; }

    public required string PictureUrl { get; init; }
}
