using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Catalog.Domain.Catalog;
using ECommerce.Modules.Catalog.IntegrationEvents;

namespace ECommerce.Modules.Catalog.Application.Catalog.UpdateItem;

internal sealed class ItemPriceChangedDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<CatalogItemPriceChangedDomainEvent>
{
    public override async Task Handle(
        CatalogItemPriceChangedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new CatalogItemPriceChangedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredAtUtc,
                domainEvent.CatalogItemId,
                domainEvent.PreviousPrice,
                domainEvent.NewPrice),
            cancellationToken);
    }
}