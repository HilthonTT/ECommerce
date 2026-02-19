using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Ticketing.Application.Abstractions.Authentication;
using ECommerce.Modules.Ticketing.Application.Orders.CancelOrder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Ticketing.Presentation.Orders;

internal sealed class CancelOrder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("orders/{orderId:guid}/cancel", async (
            Guid orderId,
            ICustomerContext customerContext,
            ICommandHandler<CancelOrderCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CancelOrderCommand(orderId, customerContext.CustomerId);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .WithTags(Tags.Orders)
        .RequireAuthorization(Permissions.CancelOrder);
    }
}
