using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Ticketing.Application.Orders.GetUserOrders;

public sealed record GetUserOrderQuery(Guid CustomerId, string? Sort, int Page, int PageSize)
    : IQuery<PaginationResult<OrderSummaryDto>>;
