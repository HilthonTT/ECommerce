using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Ticketing.Application.Abstractions.Authentication;
using ECommerce.Modules.Ticketing.Application.Carts.UpdateItemQuantity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Ticketing.Presentation.Carts;

internal sealed class UpdateCartItemQuantity : IEndpoint
{
    private sealed record Request(int ProductId, int Quantity);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("carts/update-quantity", async (
            Request request,
            ICustomerContext customerContext,
            ICommandHandler<UpdateItemQuantityCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateItemQuantityCommand(
                customerContext.CustomerId,
                request.ProductId,
                request.Quantity);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .WithTags(Tags.Carts)
        .RequireAuthorization(Permissions.AddToCart);
    }
}
