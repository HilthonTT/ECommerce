using ECommerce.Common.Application.EventBus;

namespace ECommerce.Modules.Catalog.IntegrationEvents;

public sealed class CatalogTypeCreatedIntegrationEvent : IntegrationEvent
{
    public Guid ProductTypeId { get; init; }

    public string Type { get; init; }

    public CatalogTypeCreatedIntegrationEvent(
        Guid id,
        DateTime occurredAtUtc,
        Guid productBrandId,
        string type)
        : base(id, occurredAtUtc)
    {
        ProductTypeId = productBrandId;
        Type = type;
    }
}