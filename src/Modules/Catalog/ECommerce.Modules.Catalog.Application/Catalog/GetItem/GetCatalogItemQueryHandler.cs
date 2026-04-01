using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Links;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Catalog.Application.Abstractions.Data;
using ECommerce.Modules.Catalog.Domain.Catalog;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetItem;

internal sealed class GetCatalogItemQueryHandler(IDbContext dbContext, ILinkService linkService)
    : IQueryHandler<GetCatalogItemQuery, CatalogItemResponse>
{
    public async Task<Result<CatalogItemResponse>> Handle(
        GetCatalogItemQuery query,
        CancellationToken cancellationToken)
    {
        CatalogItemResponse? response = await dbContext.CatalogItems
            .AsNoTracking()
            .Where(c => c.Id == query.Id)
            .Select(CatalogItemMappings.ProjectToResponse())
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
        {
            return CatalogItemErrors.NotFound(query.Id);
        }

        response.Links.AddRange(CreateItemLinks(query.Id));

        return response;
    }

    private List<LinkDto> CreateItemLinks(int id) =>
    [
        linkService.CreateForEndpoint(CatalogEndpointNames.GetItemById, "self",        HttpMethods.Get,    new { id }),
        linkService.CreateForEndpoint(CatalogEndpointNames.UpdateItem,  "update-item", HttpMethods.Put,    new { id }),
        linkService.CreateForEndpoint(CatalogEndpointNames.DeleteItem,  "delete-item", HttpMethods.Delete, new { id }),
    ];
}
