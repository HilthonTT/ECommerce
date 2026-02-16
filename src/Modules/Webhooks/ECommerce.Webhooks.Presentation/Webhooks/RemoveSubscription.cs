using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Webhooks.Application.Abstractions.Authentication;
using ECommerce.Webhooks.Application.Webhooks.RemoveSubscription;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Webhooks.Presentation.Webhooks;

internal sealed class RemoveSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("webhooks/subscriptions/{subscriptionId:guid}", async (
            Guid subscriptionId,
            IUserContext userContext,
            ICommandHandler<RemoveSubscriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveSubscriptionCommand(subscriptionId, userContext.UserId);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .RequireAuthorization(Permissions.RemoveWebhooks)
        .WithTags(Tags.Webhooks);
    }
}
