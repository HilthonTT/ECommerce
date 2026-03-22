using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Catalog.IntegrationEvents;
using ECommerce.Modules.Ticketing.Application.Products.CreateProductType;

namespace ECommerce.Modules.Ticketing.Presentation.Products;

internal sealed class CatalogTypeCreatedIntegrationEventHandler(ICommandHandler<CreateProductTypeCommand> handler)
    : IntegrationEventHandler<CatalogTypeCreatedIntegrationEvent>
{
    public override async Task Handle(
        CatalogTypeCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        await handler.Handle(
            new CreateProductTypeCommand(
                integrationEvent.ProductTypeId,
                integrationEvent.Type),
            cancellationToken);
    }
}
