using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Catalog.Application.Catalog;
using ECommerce.Modules.Catalog.Application.Catalog.CreateItem;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Catalog.Presentation.Endpoints;

internal sealed class CreateItem : IEndpoint
{
    private sealed record Request(
        int CatalogItemId,
        Guid CatalogBrandId,
        Guid CatalogTypeId,
        string Name,
        string? Description,
        string? PictureFileName,
        decimal Price,
        int AvailableStock,
        int RestockThreshold,
        int MaxStockThreshold);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("items", async (
            Request request,
            ICommandHandler <CreateItemCommand, CatalogItemResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateItemCommand(
                request.CatalogItemId, 
                request.CatalogBrandId, 
                request.CatalogTypeId, 
                request.Name,
                request.Description,
                request.PictureFileName, 
                request.Price,
                request.AvailableStock, 
                request.RestockThreshold, 
                request.MaxStockThreshold);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Catalog)
        .RequireAuthorization(Permissions.CreateItem);
    }
}
