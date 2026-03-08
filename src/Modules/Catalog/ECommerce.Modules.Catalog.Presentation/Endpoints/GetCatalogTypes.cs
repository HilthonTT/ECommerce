using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Catalog.Application.Catalog;
using ECommerce.Modules.Catalog.Application.Catalog.GetTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Catalog.Presentation.Endpoints;

internal sealed class GetCatalogTypes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("catalog/types", async (
            IQueryHandler<GetCatalogTypesQuery, CollectionResponse<CatalogTypeResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetCatalogTypesQuery(), cancellationToken);
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Catalog);
    }
}
