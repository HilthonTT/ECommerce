using ECommerce.Common.Application.Messaging;
using ECommerce.Webhooks.Application.Webhooks.GetSubscriptions;

namespace ECommerce.Webhooks.Application.Webhooks.GetSubscription;

public sealed record GetSubscriptionQuery(Guid SubscriptionId, Guid UserId) : IQuery<WebhookSubscriptionResponse>;
