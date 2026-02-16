using ECommerce.Common.Application.Sorting;
using ECommerce.Webhooks.Application.Webhooks.GetSubscriptions;
using ECommerce.Webhooks.Domain.Webhooks;
using System.Linq.Expressions;

namespace ECommerce.Webhooks.Application.Webhooks;

public static class WebhookSubscriptionMappings
{
    public static readonly SortMappingDefinition<WebhookSubscriptionResponse, WebhookSubscription> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(WebhookSubscriptionResponse.Id), nameof(WebhookSubscription.Id)),
            new SortMapping(nameof(WebhookSubscriptionResponse.UserId), nameof(WebhookSubscription.UserId)),
            new SortMapping(nameof(WebhookSubscriptionResponse.EventType), nameof(WebhookSubscription.EventType)),
            new SortMapping(nameof(WebhookSubscriptionResponse.WebhookUrl), nameof(WebhookSubscription.WebhookUrl)),
            new SortMapping(nameof(WebhookSubscriptionResponse.CreatedAtUtc), nameof(WebhookSubscription.CreatedAtUtc)),
        ]
    };

    public static Expression<Func<WebhookSubscription, WebhookSubscriptionResponse>> ProjectToResponse()
    {
        return subscription => new WebhookSubscriptionResponse(
            subscription.Id, 
            subscription.UserId,
            subscription.EventType, 
            subscription.WebhookUrl, 
            subscription.CreatedAtUtc);
    }

    public static WebhookSubscriptionResponse ToResponse(this WebhookSubscription subscription)
    {
        return new WebhookSubscriptionResponse(
            subscription.Id,
            subscription.UserId,
            subscription.EventType,
            subscription.WebhookUrl,
            subscription.CreatedAtUtc);
    }
}
