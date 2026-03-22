using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Catalog.IntegrationEvents;
using ECommerce.Modules.Ticketing.Application.Products.RemoveStock;

namespace ECommerce.Modules.Ticketing.Presentation.Products;

internal sealed class CatalogItemStockRemovedIntegrationEventHandler(
    ICommandHandler<RemoveProductStockCommand> handler)
    : IntegrationEventHandler<CatalogItemStockRemovedIntegrationEvent>
{
    public override async Task Handle(
        CatalogItemStockRemovedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        await handler.Handle(
            new RemoveProductStockCommand(
                integrationEvent.CatalogItemId,
                integrationEvent.QuantityRemoved),
            cancellationToken);
    }
}