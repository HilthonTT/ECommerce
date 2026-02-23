namespace ECommerce.Modules.Ticketing.IntegrationEvents;

public sealed class OrderStockItem
{
    public int ProductId { get; init; }

    public int Units { get; init; }
}