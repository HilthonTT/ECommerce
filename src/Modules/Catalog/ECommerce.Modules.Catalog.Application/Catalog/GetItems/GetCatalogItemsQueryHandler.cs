using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Links;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Application.Sorting;
using ECommerce.Common.Domain;
using ECommerce.Modules.Catalog.Application.Abstractions.Data;
using ECommerce.Modules.Catalog.Domain.Catalog;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetItems;

internal sealed class GetCatalogItemsQueryHandler(
    IDbContext dbContext,
    ISortMappingProvider sortMappingProvider,
    ILinkService linkService)
    : IQueryHandler<GetCatalogItemsQuery, PaginationResult<CatalogItemResponse>>
{
    public async Task<Result<PaginationResult<CatalogItemResponse>>> Handle(
        GetCatalogItemsQuery query,
        CancellationToken cancellationToken)
    {
        var sortMappings = sortMappingProvider.GetMappings<CatalogItemResponse, CatalogItem>();

        IQueryable<CatalogItem> catalogItemQuery = dbContext.CatalogItems.AsQueryable();

        if (query.CatalogBrandId.HasValue)
        {
            catalogItemQuery = catalogItemQuery.Where(c => c.CatalogBrandId == query.CatalogBrandId.Value);
        }

        if (query.CatalogTypeId.HasValue)
        {
            catalogItemQuery = catalogItemQuery.Where(c => c.CatalogTypeId == query.CatalogTypeId.Value);
        }

        IQueryable<CatalogItemResponse> responseQuery = !string.IsNullOrWhiteSpace(query.SearchTerm)
            ? BuildTextSearchQuery(catalogItemQuery, query.SearchTerm)
            : catalogItemQuery
                .ApplySort(query.Sort, sortMappings)
                .Select(CatalogItemMappings.ProjectToResponse());

        var result = await PaginationResult<CatalogItemResponse>.CreateAsync(
            responseQuery,
            query.Page,
            query.PageSize,
            cancellationToken);

        result.Links.AddRange(CreateLinks(query, result));

        return result;
    }

    private static IQueryable<CatalogItemResponse> BuildTextSearchQuery(
        IQueryable<CatalogItem> source,
        string searchTerm) =>
        source
            .Where(c => c.SearchVector.Matches(EF.Functions.PlainToTsQuery("english", searchTerm)))
            .OrderByDescending(c => c.SearchVector.Rank(EF.Functions.PlainToTsQuery("english", searchTerm)))
            .ThenBy(c => c.Id)
            .Select(CatalogItemMappings.ProjectToResponse());

    private List<LinkDto> CreateLinks(GetCatalogItemsQuery query, PaginationResult<CatalogItemResponse> result)
    {
        var links = new List<LinkDto>
        {
            linkService.CreateForEndpoint(
                CatalogEndpointNames.GetItems,
                "self",
                HttpMethods.Get,
                ToRouteValues(query, query.Page))
        };

        if (result.HasPreviousPage)
        {
            links.Add(linkService.CreateForEndpoint(
                CatalogEndpointNames.GetItems,
                "previous-page",
                HttpMethods.Get,
                ToRouteValues(query, query.Page - 1)));
        }

        if (result.HasNextPage)
        {
            links.Add(linkService.CreateForEndpoint(
                CatalogEndpointNames.GetItems,
                "next-page",
                HttpMethods.Get,
                ToRouteValues(query, query.Page + 1)));
        }

        return links;
    }

    private static object ToRouteValues(GetCatalogItemsQuery query, int page) =>
        new
        {
            page,
            pageSize = query.PageSize,
            q = query.SearchTerm,
            sort = query.Sort,
            catalogTypeId = query.CatalogTypeId,
            catalogBrandId = query.CatalogBrandId
        };
}
