using ECommerce.Common.Domain;

namespace ECommerce.Modules.Catalog.Domain.Catalog;

public sealed class CatalogItemStockRemovedDomainEvent(
    int catalogItemId,
    int quantityRemoved,
    int remainingStock) : DomainEvent
{
    public int CatalogItemId { get; } = catalogItemId;
    public int QuantityRemoved { get; } = quantityRemoved;
    public int RemainingStock { get; } = remainingStock;
}
