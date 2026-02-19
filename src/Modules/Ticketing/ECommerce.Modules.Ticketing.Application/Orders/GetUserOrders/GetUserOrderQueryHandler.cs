using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Application.Sorting;
using ECommerce.Common.Domain;
using ECommerce.Modules.Ticketing.Application.Abstractions.Data;
using ECommerce.Modules.Ticketing.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Ticketing.Application.Orders.GetUserOrders;

internal sealed class GetUserOrderQueryHandler(IDbContext dbContext, ISortMappingProvider sortMappingProvider) 
    : IQueryHandler<GetUserOrderQuery, PaginationResult<OrderSummaryDto>>
{
    public async Task<Result<PaginationResult<OrderSummaryDto>>> Handle(
        GetUserOrderQuery query, 
        CancellationToken cancellationToken)
    {
        SortMapping[] sortMappings = sortMappingProvider.GetMappings<OrderSummaryDto, Order>();

        IQueryable<OrderSummaryDto> ordersQuery = dbContext.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == query.CustomerId)
            .ApplySort(query.Sort, [])
            .Select(OrderMappings.ProjectToSummaryDto());

        var paginatedOrders = await PaginationResult<OrderSummaryDto>.CreateAsync(
            ordersQuery, 
            query.Page, 
            query.PageSize, 
            cancellationToken);

        return paginatedOrders;
    }
}
