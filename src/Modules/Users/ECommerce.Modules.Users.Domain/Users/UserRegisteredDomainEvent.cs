using ECommerce.Common.Domain;

namespace ECommerce.Modules.Users.Domain.Users;

public sealed class UserRegisteredDomainEvent(Guid userId) : DomainEvent
{
    public Guid UserId { get; } = userId;
}
