using ECommerce.Common.Application.EventBus;

namespace ECommerce.Modules.Catalog.IntegrationEvents;

public sealed class CatalogItemRestockThresholdReachedIntegrationEvent : IntegrationEvent
{
    public int CatalogItemId { get; init; }
    public int AvailableStock { get; init; }
    public int RestockThreshold { get; init; }

    public CatalogItemRestockThresholdReachedIntegrationEvent(
        Guid id,
        DateTime occurredAtUtc,
        int catalogItemId,
        int availableStock,
        int restockThreshold)
        : base(id, occurredAtUtc)
    {
        CatalogItemId = catalogItemId;
        AvailableStock = availableStock;
        RestockThreshold = restockThreshold;
    }
}