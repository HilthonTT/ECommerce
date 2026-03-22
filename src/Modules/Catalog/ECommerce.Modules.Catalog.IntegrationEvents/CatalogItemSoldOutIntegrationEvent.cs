using ECommerce.Common.Application.EventBus;

namespace ECommerce.Modules.Catalog.IntegrationEvents;

public sealed class CatalogItemSoldOutIntegrationEvent : IntegrationEvent
{
    public int CatalogItemId { get; init; }

    public CatalogItemSoldOutIntegrationEvent(
        Guid id,
        DateTime occurredAtUtc,
        int catalogItemId)
        : base(id, occurredAtUtc)
    {
        CatalogItemId = catalogItemId;
    }
}