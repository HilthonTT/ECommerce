using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Ticketing.Application.Abstractions.Authentication;
using ECommerce.Modules.Ticketing.Application.Orders.GetOrder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Ticketing.Presentation.Orders;

internal sealed class GetOrder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("orders/{orderId:guid}", async (
            Guid orderId,
            ICustomerContext customerContext,
            IQueryHandler<GetOrderQuery, OrderDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetOrderQuery(orderId, customerContext.CustomerId);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Orders)
        .RequireAuthorization(Permissions.ViewOrders);
    }
}
