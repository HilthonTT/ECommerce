namespace ECommerce.Modules.Users.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetAsync(Guid userId, CancellationToken cancellationToken = default);

    Task StorePendingTwoFactorSecretAsync(Guid userId, string secret, CancellationToken cancellationToken = default);

    void Insert(User user);

    Task<string?> GetTwoFactorSecretAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}