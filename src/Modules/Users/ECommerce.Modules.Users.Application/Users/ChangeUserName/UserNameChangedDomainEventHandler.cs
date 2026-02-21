using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Users.Domain.Users;
using ECommerce.Modules.Users.IntegrationEvents;

namespace ECommerce.Modules.Users.Application.Users.ChangeUserName;

internal sealed class UserNameChangedDomainEventHandler(IEventBus eventBus) : DomainEventHandler<UserNameChangedDomainEvent>
{
    public override async Task Handle(UserNameChangedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await eventBus.PublishAsync(
            new UserNameChangedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredAtUtc,
                domainEvent.UserId,
                domainEvent.FirstName,
                domainEvent.LastName),
            cancellationToken);
    }
}
