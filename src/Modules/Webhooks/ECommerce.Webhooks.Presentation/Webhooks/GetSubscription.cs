using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Webhooks.Application.Abstractions.Authentication;
using ECommerce.Webhooks.Application.Webhooks.GetSubscription;
using ECommerce.Webhooks.Application.Webhooks.GetSubscriptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Webhooks.Presentation.Webhooks;

internal sealed class GetSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("webhooks/subscriptions/{subscriptionId:guid}", async (
            Guid subscriptionId,
            IUserContext userContext,
            IQueryHandler<GetSubscriptionQuery, WebhookSubscriptionResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSubscriptionQuery(subscriptionId, userContext.UserId);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization(Permissions.ViewWebhooks)
        .WithTags(Tags.Webhooks);
    }
}
