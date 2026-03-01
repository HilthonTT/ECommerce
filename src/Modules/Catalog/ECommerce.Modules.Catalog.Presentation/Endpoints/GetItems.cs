using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Catalog.Application.Catalog;
using ECommerce.Modules.Catalog.Application.Catalog.GetItems;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Catalog.Presentation.Endpoints;

internal sealed class GetItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("catalog/items", async (
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery(Name = "q")] string? searchTerm,
            [FromQuery] string? sort,
            [FromQuery] Guid? catalogTypeId,
            [FromQuery] Guid? catalogBrandId,
            IQueryHandler<GetCatalogItemsQuery, PaginationResult<CatalogItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(
                new GetCatalogItemsQuery(page, pageSize, searchTerm, sort, catalogTypeId, catalogBrandId), 
                cancellationToken);
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Catalog);
    }
}
