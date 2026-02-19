using ECommerce.Modules.Ticketing.Domain.Orders;

namespace ECommerce.Modules.Ticketing.Application.Orders.GetUserOrders;

public sealed record OrderSummaryDto
{
    public required Guid OrderId { get; set; }

    public required DateTime CreatedAtUtc { get; set; }

    public required OrderStatus Status { get; set; }

    public required decimal TotalPrice { get; set; }

    public required string Currency { get; set; }
}