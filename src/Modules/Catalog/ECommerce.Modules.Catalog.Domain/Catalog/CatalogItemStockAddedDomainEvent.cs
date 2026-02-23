using ECommerce.Common.Domain;

namespace ECommerce.Modules.Catalog.Domain.Catalog;

public sealed class CatalogItemStockAddedDomainEvent(
    int catalogItemId,
    int quantityAdded,
    int newStock) : DomainEvent
{
    public int CatalogItemId { get; } = catalogItemId;
    public int QuantityAdded { get; } = quantityAdded;
    public int NewStock { get; } = newStock;
}
