using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Ticketing.Application.Abstractions.Authentication;
using ECommerce.Modules.Ticketing.Application.Carts.GetCart;
using ECommerce.Modules.Ticketing.Application.Orders.CreateOrderDraft;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Ticketing.Presentation.Orders;

internal sealed class CreateDraftOrder : IEndpoint
{
    private sealed record Request(List<CartItemDto> Items);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/orders/draft", async (
            Request request,
            ICustomerContext customerContext,
            ICommandHandler<CreateOrderDraftCommand, OrderDraftDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateOrderDraftCommand(customerContext.CustomerId, request.Items);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization(Permissions.CreateOrder)
        .WithTags(Tags.Orders);
    }
}
