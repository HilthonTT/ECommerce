using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Products;

public sealed class ProductBrandCreatedDomainEvent(Guid productBrandId, string brand) : DomainEvent
{
    public Guid ProductBrandId { get; } = productBrandId;
    public string Brand { get; } = brand;
}
