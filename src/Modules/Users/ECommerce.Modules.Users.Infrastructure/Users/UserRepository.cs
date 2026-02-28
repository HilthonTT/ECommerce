using ECommerce.Common.Application.Encryption;
using ECommerce.Modules.Users.Domain.Users;
using ECommerce.Modules.Users.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Users.Infrastructure.Users;

internal sealed class UserRepository(UsersDbContext dbContext, IEncryptionService encryptionService) : IUserRepository
{
    public Task<User?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public void Insert(User user)
    {
        dbContext.Users.Add(user);
    }

    public async Task StorePendingTwoFactorSecretAsync(
        Guid userId, 
        string secret, 
        CancellationToken cancellationToken = default)
    {
        EncryptionResult encryptionResult = encryptionService.Encrypt(secret);

        await dbContext.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(u => u
                .SetProperty(x => x.PendingTwoFactorSecret, encryptionResult.EncryptedData)
                .SetProperty(x => x.PendingTwoFactorSecretKey, encryptionResult.Key),
                cancellationToken);
    }

    public async Task<string?> GetTwoFactorSecretAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        User? user = await dbContext.Users
         .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null || user.PendingTwoFactorSecret is null || user.PendingTwoFactorSecretKey is null)
        {
            return null;
        }

        var encryptionResult = new EncryptionResult
        {
            EncryptedData = user.PendingTwoFactorSecret,
            Key = user.PendingTwoFactorSecretKey
        };

        return encryptionService.Decrypt(encryptionResult);
    }
}
