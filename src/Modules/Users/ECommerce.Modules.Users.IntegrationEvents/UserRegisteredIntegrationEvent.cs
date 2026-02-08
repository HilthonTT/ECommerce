using ECommerce.Common.Application.EventBus;

namespace ECommerce.Modules.Users.IntegrationEvents;

public sealed class UserRegisteredIntegrationEvent(
    Guid id,
    DateTime occurredAtUtc,
    Guid userId,
    string email,
    string firstName,
    string lastName)
    : IntegrationEvent(id, occurredAtUtc)
{
    public Guid UserId { get; } = userId;
    public string Email { get; } = email;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
}
