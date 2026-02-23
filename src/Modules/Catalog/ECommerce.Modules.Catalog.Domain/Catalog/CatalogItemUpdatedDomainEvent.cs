using ECommerce.Common.Domain;

namespace ECommerce.Modules.Catalog.Domain.Catalog;

public sealed class CatalogItemUpdatedDomainEvent(
    int catalogItemId,
    string name,
    string? description,
    decimal price) : DomainEvent
{
    public int CatalogItemId { get; } = catalogItemId;
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public decimal Price { get; } = price;
}
