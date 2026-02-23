using ECommerce.Common.Domain;

namespace ECommerce.Modules.Catalog.Domain.Catalog;

public sealed class CatalogItemPriceChangedDomainEvent(
    int catalogItemId,
    decimal previousPrice,
    decimal newPrice) : DomainEvent
{
    public int CatalogItemId { get; } = catalogItemId;
    public decimal PreviousPrice { get; } = previousPrice;
    public decimal NewPrice { get; } = newPrice;
}
