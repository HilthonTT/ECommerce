using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Links;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Catalog.Application.Abstractions.Data;
using ECommerce.Modules.Catalog.Domain.Catalog;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Modules.Catalog.Application.Catalog.CreateItem;

internal sealed class CreateItemCommandHandler(
    ICatalogItemRepository catalogItemRepository,
    IUnitOfWork unitOfWork,
    ILinkService linkService) : ICommandHandler<CreateItemCommand, CatalogItemResponse>
{
    public async Task<Result<CatalogItemResponse>> Handle(
        CreateItemCommand command, 
        CancellationToken cancellationToken)
    {
        var catalogItem = CatalogItem.Create(
            command.CatalogItemId,
            command.Name,
            command.Description,
            command.Price,
            command.CatalogTypeId,
            command.CatalogBrandId,
            command.AvailableStock,
            command.RestockThreshold,
            command.MaxStockThreshold,
            command.PictureFileName);

        catalogItemRepository.Insert(catalogItem);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        CatalogItemResponse response = catalogItem.ToResponse();
        response.Links.AddRange(CreateItemLinks(catalogItem.Id));

        return response;
    }

    private List<LinkDto> CreateItemLinks(int id) =>
    [
        linkService.CreateForEndpoint(CatalogEndpointNames.GetItemById, "self",        HttpMethods.Get,    new { id }),
        linkService.CreateForEndpoint(CatalogEndpointNames.UpdateItem,  "update-item", HttpMethods.Put,    new { id }),
        linkService.CreateForEndpoint(CatalogEndpointNames.DeleteItem,  "delete-item", HttpMethods.Delete, new { id }),
    ];
}
