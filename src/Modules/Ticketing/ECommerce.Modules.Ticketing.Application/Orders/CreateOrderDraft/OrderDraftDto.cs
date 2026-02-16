namespace ECommerce.Modules.Ticketing.Application.Orders.CreateOrderDraft;

public sealed record OrderDraftDto
{
    public List<OrderItemDto> OrderItems { get; init; } = [];

    public required decimal Total { get; init; }
}
