using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Ticketing.Application.Abstractions.Authentication;
using ECommerce.Modules.Ticketing.Application.Carts.AddItemToCart;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Ticketing.Presentation.Carts;

internal sealed class AddToCart : IEndpoint
{
    private sealed record Request(Guid ProductId, int Quantity);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("carts/add", async (
            Request request,
            ICustomerContext customerContext,
            ICommandHandler<AddItemToCartCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddItemToCartCommand(customerContext.CustomerId, request.ProductId, request.Quantity);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .WithTags(Tags.Carts)
        .RequireAuthorization(Permissions.AddToCart);
    }
}
