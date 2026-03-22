using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Catalog.Domain.Catalog;
using ECommerce.Modules.Catalog.IntegrationEvents;

namespace ECommerce.Modules.Catalog.Application.Catalog.UpdateItem;

internal sealed class ItemStockRemovedDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<CatalogItemStockRemovedDomainEvent>
{
    public override async Task Handle(
        CatalogItemStockRemovedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new CatalogItemStockRemovedIntegrationEvent(
                Guid.NewGuid(),
                domainEvent.OccurredAtUtc,
                domainEvent.CatalogItemId,
                domainEvent.QuantityRemoved,
                domainEvent.AvailableStock),
            cancellationToken);
    }
}