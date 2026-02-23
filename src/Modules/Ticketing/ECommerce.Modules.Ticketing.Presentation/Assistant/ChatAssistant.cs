using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Ticketing.Application.Assistant.GetStreamingChatResponse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Ticketing.Presentation.Assistant;

internal sealed class ChatAssistant : IEndpoint
{
    private sealed record Request(
        int? ProductId,
        string? CustomerName,
        string? TicketSummary,
        string? TicketLastCustomerMessage,
        IReadOnlyList<GetStreamChatResponseCommand.AssistantChatRequestMessage> Messages);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("assistant/chat", async (
            Request request,
            ICommandHandler<GetStreamChatResponseCommand> handler,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            httpContext.Response.ContentType = "application/x-ndjson";
            httpContext.Response.Headers.Append("Cache-Control", "no-cache");
            httpContext.Response.Headers.Append("X-Accel-Buffering", "no");

            var command = new GetStreamChatResponseCommand(
                request.ProductId,
                request.CustomerName,
                request.TicketSummary,
                request.TicketLastCustomerMessage,
                request.Messages);

            Result result = await handler.Handle(command, cancellationToken);

            // Don't return anything - response has already been written
            // If there was an error, it should have been handled in the handler
        })
        .WithTags(Tags.Assistant)
        .RequireAuthorization(Permissions.ChatAssistant);
    }
}
