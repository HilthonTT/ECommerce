using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Ticketing.Application.Abstractions.Authentication;
using ECommerce.Modules.Ticketing.Application.Tickets.CreateTicket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Ticketing.Presentation.Tickets;

internal sealed class CreateTicket : IEndpoint
{
    private sealed record Request(int? ProductId, string Message);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/tickets", async (
            Request request,
            ICustomerContext customerContext,
            ICommandHandler<CreateTicketCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateTicketCommand(customerContext.CustomerId, request.ProductId, request.Message);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .WithTags(Tags.Tickets)
        .RequireAuthorization(Permissions.CreateTicket);
    }
}
