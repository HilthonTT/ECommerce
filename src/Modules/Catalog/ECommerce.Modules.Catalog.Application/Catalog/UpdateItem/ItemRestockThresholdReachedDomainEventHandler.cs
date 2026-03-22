using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Catalog.Domain.Catalog;
using ECommerce.Modules.Catalog.IntegrationEvents;

namespace ECommerce.Modules.Catalog.Application.Catalog.UpdateItem;

internal sealed class ItemRestockThresholdReachedDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<CatalogItemRestockThresholdReachedDomainEvent>
{
    public override async Task Handle(
        CatalogItemRestockThresholdReachedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new CatalogItemRestockThresholdReachedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredAtUtc,
                domainEvent.CatalogItemId,
                domainEvent.CurrentStock,
                domainEvent.RestockThreshold),
            cancellationToken);
    }
}