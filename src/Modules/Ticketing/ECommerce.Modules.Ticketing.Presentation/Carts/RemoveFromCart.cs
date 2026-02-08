using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Ticketing.Application.Abstractions.Authentication;
using ECommerce.Modules.Ticketing.Application.Carts.RemoveItemFromCart;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Ticketing.Presentation.Carts;

internal sealed class RemoveFromCart : IEndpoint
{
    private sealed record Request(Guid ProductId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("carts/remove", async (
            Request request,
            ICustomerContext customerContext,
            ICommandHandler<RemoveItemFromCartCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveItemFromCartCommand(customerContext.CustomerId, request.ProductId);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .WithTags(Tags.Carts)
        .RequireAuthorization(Permissions.RemoveFromCart);
    }
}
