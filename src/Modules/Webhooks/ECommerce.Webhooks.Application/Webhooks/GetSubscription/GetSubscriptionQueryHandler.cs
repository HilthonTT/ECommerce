using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Webhooks.Application.Webhooks.GetSubscriptions;
using ECommerce.Webhooks.Domain.Users;
using ECommerce.Webhooks.Domain.Webhooks;

namespace ECommerce.Webhooks.Application.Webhooks.GetSubscription;

internal sealed class GetSubscriptionQueryHandler(IWebhookSubscriptionRepository subscriptionRepository) 
    : IQueryHandler<GetSubscriptionQuery, WebhookSubscriptionResponse>
{
    public async Task<Result<WebhookSubscriptionResponse>> Handle(GetSubscriptionQuery query, CancellationToken cancellationToken)
    {
        WebhookSubscription? subscription = await subscriptionRepository.GetByIdAsync(
            query.SubscriptionId,
            cancellationToken);

        if (subscription is null)
        {
            return WebhookErrors.NotFound(query.SubscriptionId);
        }

        if (subscription.UserId != query.UserId)
        {
            return UserErrors.Unauthorized;
        }

        return subscription.ToResponse();
    }
}
