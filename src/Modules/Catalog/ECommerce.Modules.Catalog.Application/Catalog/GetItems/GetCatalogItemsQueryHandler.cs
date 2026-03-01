using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Application.Sorting;
using ECommerce.Common.Domain;
using ECommerce.Modules.Catalog.Application.Abstractions.Data;
using ECommerce.Modules.Catalog.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace ECommerce.Modules.Catalog.Application.Catalog.GetItems;

internal sealed class GetCatalogItemsQueryHandler(
    IDbContext dbContext,
    ISortMappingProvider sortMappingProvider)
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

        return await PaginationResult<CatalogItemResponse>.CreateAsync(
            responseQuery,
            query.Page,
            query.PageSize,
            cancellationToken);
    }

    private static IQueryable<CatalogItemResponse> BuildTextSearchQuery(
        IQueryable<CatalogItem> source,
        string searchTerm) =>
        source
            .Where(c => c.SearchVector.Matches(EF.Functions.PlainToTsQuery("english", searchTerm)))
            .OrderByDescending(c => c.SearchVector.Rank(EF.Functions.PlainToTsQuery("english", searchTerm)))
            .ThenBy(c => c.Id)
            .Select(CatalogItemMappings.ProjectToResponse());
}
