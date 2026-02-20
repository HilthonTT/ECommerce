using ECommerce.Common.Domain;

namespace ECommerce.Modules.Users.Domain.Users;

public sealed class UserNameChangedDomainEvent(Guid userId, string firstName, string lastName) : DomainEvent
{
    public Guid UserId { get; } = userId;

    public string FirstName { get; } = firstName;

    public string LastName { get; } = lastName;
}
