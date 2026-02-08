using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Ticketing.Application.Abstractions.Authentication;
using ECommerce.Modules.Ticketing.Application.Tickets.GetTickets;
using ECommerce.Modules.Ticketing.Domain.Tickets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Ticketing.Presentation.Tickets;

internal sealed class GetTickets : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("tickets", async (
            [FromQuery] TicketStatus status,
            [FromQuery] string? sort,
            [FromQuery] int startIndex,
            [FromQuery] int maxResults,
            ICustomerContext customerContext,
            IQueryHandler<GetTicketsQuery, GetTicketsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetTicketsQuery(customerContext.CustomerId, [], status, sort, startIndex, maxResults);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Tickets)
        .RequireAuthorization(Permissions.ViewTickets);
    }
}
