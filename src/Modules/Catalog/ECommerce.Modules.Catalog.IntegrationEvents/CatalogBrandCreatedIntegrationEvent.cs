using ECommerce.Common.Application.EventBus;

namespace ECommerce.Modules.Catalog.IntegrationEvents;

public sealed class CatalogBrandCreatedIntegrationEvent : IntegrationEvent
{
    public Guid ProductBrandId { get; init; }

    public string Brand { get; init; }

    public CatalogBrandCreatedIntegrationEvent(
        Guid id, 
        DateTime occurredAtUtc,
        Guid productBrandId,
        string brand) 
        : base(id, occurredAtUtc)
    {
        ProductBrandId = productBrandId;
        Brand = brand;
    }
}
