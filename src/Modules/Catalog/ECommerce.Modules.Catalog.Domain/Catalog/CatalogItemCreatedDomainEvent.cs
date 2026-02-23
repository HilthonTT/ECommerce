using ECommerce.Common.Domain;

namespace ECommerce.Modules.Catalog.Domain.Catalog;

public sealed class CatalogItemCreatedDomainEvent(
    int catalogItemId,
    string name,
    decimal price,
    Guid catalogTypeId,
    Guid catalogBrandId) : DomainEvent
{
    public int CatalogItemId { get; } = catalogItemId;
    public string Name { get; } = name;
    public decimal Price { get; } = price;
    public Guid CatalogTypeId { get; } = catalogTypeId;
    public Guid CatalogBrandId { get; } = catalogBrandId;
}
