using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public sealed class CatalogTypeCreatedDomainEvent(Guid catalogTypeId, string type) : DomainEvent
{
    public Guid CatalogTypeId { get; } = catalogTypeId;
    public string Type { get; } = type;
}
