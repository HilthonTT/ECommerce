using ECommerce.Common.Application.Clock;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Webhooks.Application.Abstractions.Data;
using ECommerce.Webhooks.Application.Webhooks.GetSubscriptions;
using ECommerce.Webhooks.Domain.Webhooks;

namespace ECommerce.Webhooks.Application.Webhooks.CreateSubscription;

internal sealed class CreateSubscriptionCommandHandler(
    IWebhookSubscriptionRepository webhookSubscriptionRepository, 
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<CreateSubscriptionCommand, WebhookSubscriptionResponse>
{
    public async Task<Result<WebhookSubscriptionResponse>> Handle(
        CreateSubscriptionCommand command, 
        CancellationToken cancellationToken)
    {
        bool exists = await webhookSubscriptionRepository.ExistsAsync(
            command.UserId,
            command.EventType, 
            command.WebhookUrl,
            cancellationToken);

        if (exists)
        {
            return WebhookErrors.AlreadyExists;
        }

        var subscription = new WebhookSubscription(
            Guid.CreateVersion7(),
            command.UserId,
            command.EventType,
            command.WebhookUrl,
            dateTimeProvider.UtcNow);

        webhookSubscriptionRepository.Insert(subscription);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return subscription.ToResponse();
    }
}
