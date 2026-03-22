using ECommerce.Web.Shared.DTOs.Authentication;

namespace ECommerce.Web.Shared.Services.Authentication;

public sealed class TokenStore
{
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public AccessTokensDto? Tokens { get; set; }
    public bool IsInitialized { get; set; }

    /// <summary>
    /// Ensures only one refresh runs at a time across concurrent requests.
    /// </summary>
    public async Task<T> WithRefreshLockAsync<T>(Func<Task<T>> action)
    {
        await _refreshLock.WaitAsync();
        try
        {
            return await action();
        }
        finally
        {
            _refreshLock.Release();
        }
    }
}
