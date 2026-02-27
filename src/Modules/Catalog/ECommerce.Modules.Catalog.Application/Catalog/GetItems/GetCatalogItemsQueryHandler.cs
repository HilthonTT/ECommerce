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
        IQueryable<CatalogItem> catalogItemQuery = dbContext.CatalogItems.AsQueryable();
        bool usingTextSearch = false;

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var tsQuery = EF.Functions.PhraseToTsQuery("english", query.SearchTerm);

            catalogItemQuery = catalogItemQuery
                .Where(c => c.SearchVector.Matches(tsQuery));

            usingTextSearch = true;
        }

        if (query.CatalogBrandId.HasValue)
        {
            catalogItemQuery = catalogItemQuery.Where(c => c.CatalogBrandId == query.CatalogBrandId.Value);
        }

        if (query.CatalogTypeId.HasValue)
        {
            catalogItemQuery = catalogItemQuery.Where(c => c.CatalogTypeId == query.CatalogTypeId.Value);
        }

        var sortMappings = sortMappingProvider.GetMappings<CatalogItemResponse, CatalogItem>();

        IQueryable<CatalogItemResponse> responseQuery;

        if (usingTextSearch)
        {
            var tsQuery = EF.Functions.PhraseToTsQuery("english", query.SearchTerm!);

            responseQuery = catalogItemQuery
                .OrderByDescending(c => c.SearchVector.Rank(tsQuery))
                .Select(CatalogItemMappings.ProjectToResponse());
        }
        else
        {
            responseQuery = catalogItemQuery
                .ApplySort(query.Sort, sortMappings)
                .Select(CatalogItemMappings.ProjectToResponse());
        }

        var paginationResult = await PaginationResult<CatalogItemResponse>.CreateAsync(
            responseQuery,
            query.Page,
            query.PageSize,
            cancellationToken);

        return paginationResult;
    }
}
