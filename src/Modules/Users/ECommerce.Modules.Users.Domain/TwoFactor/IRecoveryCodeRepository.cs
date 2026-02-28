namespace ECommerce.Modules.Users.Domain.TwoFactor;

public interface IRecoveryCodeRepository
{
    /// <summary>Deletes existing codes, hashes and inserts the new plain text codes.</summary>
    Task ReplaceAsync(Guid userId, List<string> plainCodes, CancellationToken cancellationToken = default);

    /// <summary>Finds a matching unused code, marks it used, and returns true.</summary>
    Task<bool> TryConsumeAsync(Guid userId, string code, CancellationToken cancellationToken = default);
}
