using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Webhooks.Application.Abstractions.Data;
using ECommerce.Webhooks.Domain.Users;
using ECommerce.Webhooks.Domain.Webhooks;

namespace ECommerce.Webhooks.Application.Webhooks.RemoveSubscription;

internal sealed class RemoveSubscriptionCommandHandler(
    IWebhookSubscriptionRepository subscriptionRepository, 
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveSubscriptionCommand>
{
    public async Task<Result> Handle(
        RemoveSubscriptionCommand command, 
        CancellationToken cancellationToken)
    {
        WebhookSubscription? subscription = await subscriptionRepository.GetByIdAsync(
            command.SubscriptionId, 
            cancellationToken);

        if (subscription is null)
        {
            return WebhookErrors.NotFound(command.SubscriptionId);
        }

        if (subscription.UserId != command.UserId)
        {
            return UserErrors.Unauthorized;
        }

        subscriptionRepository.Remove(subscription);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
