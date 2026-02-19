using ECommerce.Modules.Ticketing.Domain.Orders;

namespace ECommerce.Modules.Ticketing.Application.Orders.GetOrder;

public sealed record OrderDto
{
    public Guid Id { get; init; }

    public required Guid CustomerId { get; set; }

    public required OrderStatus Status { get; set; }

    public required DateTime CreatedAtUtc { get; set; }

    public required decimal TotalPrice { get; set; }

    public required string Description { get; set; }

    public required string Currency { get; set; }

    public required AddressDto Address { get; set; }
}
