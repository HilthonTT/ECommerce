using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Ticketing.Application.Abstractions.Authentication;
using ECommerce.Modules.Ticketing.Application.Orders.GetUserOrders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Ticketing.Presentation.Orders;

internal sealed class GetUserOrders : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("orders", async (
            [FromQuery] string? sort,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            ICustomerContext customerContext,
            IQueryHandler<GetUserOrderQuery, PaginationResult<OrderSummaryDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserOrderQuery(customerContext.CustomerId, sort, page, pageSize);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Orders)
        .RequireAuthorization(Permissions.ViewOrders);
    }
}
