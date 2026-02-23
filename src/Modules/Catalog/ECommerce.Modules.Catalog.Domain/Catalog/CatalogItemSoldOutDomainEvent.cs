using ECommerce.Common.Domain;

namespace ECommerce.Modules.Catalog.Domain.Catalog;

public sealed class CatalogItemSoldOutDomainEvent(int catalogItemId) : DomainEvent
{
    public int CatalogItemId { get; } = catalogItemId;
}