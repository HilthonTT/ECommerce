using ECommerce.Common.Domain;
using ECommerce.Common.Domain.Auditing;

namespace ECommerce.Modules.Users.Domain.Users;

[Auditable]
public sealed class User : Entity
{
    private readonly List<Role> _roles = [];

    public Guid Id { get; private set; }

    public string Email { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string IdentityId { get; private set; }

    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    public bool TwoFactorEnabled { get; private set; }

    public string? PendingTwoFactorSecret { get; private set; }

    public string? PendingTwoFactorSecretKey { get; private set; }

    public string? TwoFactorSecret { get; private set; }

    public string? TwoFactorSecretKey { get; private set; }

    public long? LastUsedTimeStep { get; private set; }

    private User()
    {
    }

    public static User Create(
       string email,
       string firstName,
       string lastName,
       string identityId)
    {
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            IdentityId = identityId
        };

        user._roles.Add(Role.Member);

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id));

        return user;
    }

    public void ChangeName(string firstName, string lastName)
    {
        if (FirstName == firstName && LastName == lastName)
        {
            return;
        }

        FirstName = firstName;
        LastName = lastName;

        RaiseDomainEvent(new UserNameChangedDomainEvent(Id, FirstName, LastName));
    }

    public void SetPendingTwoFactorSecret(string encryptedData, string key)
    {
        PendingTwoFactorSecret = encryptedData;
        PendingTwoFactorSecretKey = key;
    }

    public void ActivateTwoFactor()
    {
        if (PendingTwoFactorSecret is null || PendingTwoFactorSecretKey is null)
        {
            throw new InvalidOperationException("No pending 2FA secret to activate.");
        }

        TwoFactorSecret = PendingTwoFactorSecret;
        TwoFactorSecretKey = PendingTwoFactorSecretKey;
        PendingTwoFactorSecret = null;
        PendingTwoFactorSecretKey = null;
        TwoFactorEnabled = true;
    }

    public void UpdateLastUsedTimeStep(long timeStep)
    {
        LastUsedTimeStep = timeStep;
    }

    public void DisableTwoFactor()
    {
        TwoFactorEnabled = false;
        TwoFactorSecret = null;
        TwoFactorSecretKey = null;
        LastUsedTimeStep = null;
    }
}
