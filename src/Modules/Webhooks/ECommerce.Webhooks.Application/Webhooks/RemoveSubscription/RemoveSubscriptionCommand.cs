using ECommerce.Common.Application.Messaging;

namespace ECommerce.Webhooks.Application.Webhooks.RemoveSubscription;

public sealed record RemoveSubscriptionCommand(Guid SubscriptionId, Guid UserId) : ICommand;
