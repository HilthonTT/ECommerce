using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public sealed class ProductTypeCreatedDomainEvent(Guid catalogTypeId, string type) : DomainEvent
{
    public Guid CatalogTypeId { get; } = catalogTypeId;
    public string Type { get; } = type;
}
