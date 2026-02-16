using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Webhooks.Application.Abstractions.Authentication;
using ECommerce.Webhooks.Application.Webhooks.CreateSubscription;
using ECommerce.Webhooks.Application.Webhooks.GetSubscriptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Webhooks.Presentation.Webhooks;

internal sealed class CreateSubscription : IEndpoint
{
    private sealed record Request(string EventType, string WebhookUrl);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("webhooks/subscription", async (
            Request request,
            ICommandHandler <CreateSubscriptionCommand, WebhookSubscriptionResponse> handler,
            IUserContext userContext,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateSubscriptionCommand(userContext.UserId, request.EventType, request.WebhookUrl);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization(Permissions.CreateWebhooks)
        .WithTags(Tags.Webhooks);
    }
}
