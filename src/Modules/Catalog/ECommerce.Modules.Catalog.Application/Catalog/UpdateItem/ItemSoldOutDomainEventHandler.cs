using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Catalog.Domain.Catalog;
using ECommerce.Modules.Catalog.IntegrationEvents;

namespace ECommerce.Modules.Catalog.Application.Catalog.UpdateItem;

internal sealed class ItemSoldOutDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<CatalogItemSoldOutDomainEvent>
{
    public override async Task Handle(
        CatalogItemSoldOutDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new CatalogItemSoldOutIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredAtUtc,
                domainEvent.CatalogItemId),
            cancellationToken);
    }
}