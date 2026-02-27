using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Ticketing.Application.Tickets.UpdateTicket;
using ECommerce.Modules.Ticketing.Domain.Tickets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Ticketing.Presentation.Tickets;

internal sealed class UpdateTicket : IEndpoint
{
    private sealed record Request(int? ProductId, TicketType Type, TicketStatus Status);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("tickets/{ticketId:guid}", async (
            Request request,
            Guid ticketId,
            ICommandHandler<UpdateTicketCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateTicketCommand(ticketId, request.ProductId, request.Type, request.Status);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .WithTags(Tags.Tickets)
        .RequireAuthorization(Permissions.UpdateTickets);
    }
}
