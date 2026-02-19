using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Ticketing.Application.Abstractions.Authentication;
using ECommerce.Modules.Ticketing.Application.Orders.CreateOrder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Ticketing.Presentation.Orders;

internal sealed class CreateOrder : IEndpoint
{
    private sealed record Request(
        string City,
        string Street,
        string State,
        string Country,
        string ZipCode,
        string CardNumber,
        string CardHolderName,
        DateTime CardExpiration,
        string CardSecurityNumber);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("orders", async (
            Request request,
            ICustomerContext customerContext,
            ICommandHandler<CreateOrderCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateOrderCommand(
                customerContext.CustomerId,
                request.City,
                request.Street,
                request.State,
                request.Country,
                request.ZipCode,
                request.CardNumber,
                request.CardHolderName,
                request.CardExpiration,
                request.CardSecurityNumber);

            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .WithTags(Tags.Orders)
        .RequireCors(Permissions.CreateOrder);
    }
}
