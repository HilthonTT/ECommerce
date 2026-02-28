using ECommerce.Common.Domain;

namespace ECommerce.Modules.Users.Domain.TwoFactor;

public sealed class RecoveryCode : Entity
{
    public Guid Id { get; private init; }

    public Guid UserId { get; private set; }

    public string CodeHash { get; private set; } = string.Empty;

    public bool IsUsed { get; private set; }

    private RecoveryCode()
    {
    }

    public static RecoveryCode Create(Guid userId, string codeHash)
    {
        return new RecoveryCode
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            CodeHash = codeHash
        };
    }

    public void MarkUsed() => IsUsed = true;
}
