using ECommerce.Common.Application.EventBus;

namespace ECommerce.Modules.Catalog.IntegrationEvents;

public sealed class CatalogItemCreatedIntegrationEvent : IntegrationEvent
{
    public int CatalogItemId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public Guid CatalogTypeId { get; init; }
    public Guid CatalogBrandId { get; init; }
    public int AvailableStock { get; init; }
    public int RestockThreshold { get; init; }
    public int MaxStockThreshold { get; init; }

    public CatalogItemCreatedIntegrationEvent(
        Guid id,
        DateTime occurredAtUtc,
        int catalogItemId,
        string name,
        decimal price,
        Guid catalogTypeId,
        Guid catalogBrandId,
        int availableStock,
        int restockThreshold,
        int maxStockThreshold)
        : base(id, occurredAtUtc)
    {
        CatalogItemId = catalogItemId;
        Name = name;
        Price = price;
        CatalogTypeId = catalogTypeId;
        CatalogBrandId = catalogBrandId;
        AvailableStock = availableStock;
        RestockThreshold = restockThreshold;
        MaxStockThreshold = maxStockThreshold;
    }
}
