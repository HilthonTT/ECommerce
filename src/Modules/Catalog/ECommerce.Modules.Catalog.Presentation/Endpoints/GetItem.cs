using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Catalog.Application.Catalog;
using ECommerce.Modules.Catalog.Application.Catalog.GetItem;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Catalog.Presentation.Endpoints;

internal sealed class GetItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("catalog/items/{id:int}", async (
            int id,
            IQueryHandler<GetCatalogItemQuery, CatalogItemResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetCatalogItemQuery(id), cancellationToken);
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Catalog);
    }
}
