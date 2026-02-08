using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Ticketing.Application.Abstractions.Authentication;
using ECommerce.Modules.Ticketing.Application.Carts.GetCart;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Ticketing.Presentation.Carts;

internal sealed class GetCart : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("carts", async (
            ICustomerContext customerContext,
            IQueryHandler<GetCartQuery, CartDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCartQuery(customerContext.CustomerId);

            var result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Carts)
        .RequireAuthorization(Permissions.ViewCart);
    }
}
