using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Catalog.IntegrationEvents;
using ECommerce.Modules.Ticketing.Application.Products.CreateProductBrand;

namespace ECommerce.Modules.Ticketing.Presentation.Products;

internal sealed class CatalogBrandCreatedIntegrationEventHandler(ICommandHandler<CreateProductBrandCommand> handler) 
    : IntegrationEventHandler<CatalogBrandCreatedIntegrationEvent>
{
    public override async Task Handle(
        CatalogBrandCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        await handler.Handle(
            new CreateProductBrandCommand(
                integrationEvent.Id,
                integrationEvent.Brand),
            cancellationToken);
    }
}
