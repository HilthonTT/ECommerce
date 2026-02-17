namespace ECommerce.Modules.Ticketing.IntegrationEvents;

public sealed class OrderStockItem
{
    public Guid ProductId { get; init; }

    public int Units { get; init; }
}