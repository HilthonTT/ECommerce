using ECommerce.Common.Domain;

namespace ECommerce.Modules.Catalog.Domain.Catalog;

public sealed class CatalogItemRestockThresholdReachedDomainEvent(
    int catalogItemId,
    int currentStock,
    int restockThreshold) : DomainEvent
{
    public int CatalogItemId { get; } = catalogItemId;
    public int CurrentStock { get; } = currentStock;
    public int RestockThreshold { get; } = restockThreshold;
}
