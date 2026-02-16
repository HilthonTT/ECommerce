using ECommerce.Common.Application.Messaging;
using ECommerce.Webhooks.Application.Webhooks.GetSubscriptions;

namespace ECommerce.Webhooks.Application.Webhooks.CreateSubscription;

public sealed record CreateSubscriptionCommand(Guid UserId, string EventType, string WebhookUrl) 
    : ICommand<WebhookSubscriptionResponse>;
