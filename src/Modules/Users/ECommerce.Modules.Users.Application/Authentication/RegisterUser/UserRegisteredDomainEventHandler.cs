using ECommerce.Common.Application.EventBus;
using ECommerce.Common.Application.Exceptions;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Application.Users;
using ECommerce.Modules.Users.Application.Users.GetUser;
using ECommerce.Modules.Users.Domain.Users;
using ECommerce.Modules.Users.IntegrationEvents;

namespace ECommerce.Modules.Users.Application.Authentication.RegisterUser;

internal sealed class UserRegisteredDomainEventHandler(
    IEventBus eventBus,
    IQueryHandler<GetUserQuery, UserResponse> handler) : DomainEventHandler<UserRegisteredDomainEvent>
{
    public override async Task Handle(UserRegisteredDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Result<UserResponse> result = await handler.Handle(new GetUserQuery(domainEvent.UserId), cancellationToken);
        if (result.IsFailure)
        {
            throw new ECommerceException(nameof(GetUserQuery), result.Error);
        }

        UserResponse user = result.Value;

        await eventBus.PublishAsync(
            new UserRegisteredIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredAtUtc,
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName),
            cancellationToken);
    }
}
