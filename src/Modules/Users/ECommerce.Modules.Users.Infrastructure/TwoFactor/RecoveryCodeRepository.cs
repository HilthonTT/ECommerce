using ECommerce.Modules.Users.Domain.TwoFactor;
using ECommerce.Modules.Users.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Modules.Users.Infrastructure.TwoFactor;

internal sealed class RecoveryCodeRepository(UsersDbContext dbContext) : IRecoveryCodeRepository
{
    public async Task ReplaceAsync(Guid userId, List<string> plainCodes, CancellationToken cancellationToken = default)
    {
        // Delete all existing codes for this user
        await dbContext.RecoveryCodes
            .Where(rc => rc.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        // Hash and insert new codes
        IEnumerable<RecoveryCode> hashed = plainCodes
            .Select(code => RecoveryCode.Create(userId, BCrypt.Net.BCrypt.HashPassword(code)));

        dbContext.RecoveryCodes.AddRange(hashed);
    }

    public async Task<bool> TryConsumeAsync(Guid userId, string code, CancellationToken cancellationToken = default)
    {
        List<RecoveryCode> stored = await dbContext.RecoveryCodes
             .Where(rc => rc.UserId == userId && !rc.IsUsed)
             .ToListAsync(cancellationToken);

        var match = stored
            .FirstOrDefault(rc => BCrypt.Net.BCrypt.Verify(code, rc.CodeHash));

        if (match is null)
        {
            return false;
        }

        match.MarkUsed();

        return true;
    }
}
