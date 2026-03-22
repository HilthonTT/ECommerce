using ECommerce.Web.Shared.DTOs.Authentication;
using ECommerce.Web.Shared.Services.Authentication.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.Web.Shared.Services.Authentication;

public sealed class TokenAuthStateProvider(
    IAuthenticationService authenticationService,
    ProtectedLocalStorage localStorage,
    TokenStore tokenStore) : AuthenticationStateProvider
{
    private static readonly JwtSecurityTokenHandler JwtHandler = new();

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await InitializeAsync();

        if (tokenStore.Tokens is null)
        {
            return Anonymous();
        }

        var jwt = JwtHandler.ReadJwtToken(tokenStore.Tokens.AccessToken);

        if (jwt.ValidTo < DateTime.UtcNow)
        {
            var refreshed = await tokenStore.WithRefreshLockAsync(TryRefreshAsync);

            if (!refreshed)
            {
                return Anonymous();
            }

            jwt = JwtHandler.ReadJwtToken(tokenStore.Tokens.AccessToken);
        }

        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task<LoginDto?> LoginAsync(string email, string password)
    {
        var result = await authenticationService.LoginAsync(email, password);

        if (result?.AccessTokens is not null)
        {
            await PersistTokensAsync(result.AccessTokens);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        return result;
    }

    public async Task LogoutAsync()
    {
        await ClearTokensAsync();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private async Task InitializeAsync()
    {
        if (tokenStore.IsInitialized)
        {
            return;
        }

        tokenStore.IsInitialized = true;

        try
        {
            var result = await localStorage.GetAsync<AccessTokensDto>(AuthCacheKeys.TokenKey);
            if (result.Success && result.Value is not null)
            {
                tokenStore.Tokens = result.Value;
            }
        }
        catch (InvalidOperationException)
        {
            // Prerendering — no JS interop
        }
    }

    private async Task<bool> TryRefreshAsync()
    {
        // Double-check after acquiring the lock — another call may have already refreshed
        if (tokenStore.Tokens is not null)
        {
            var current = JwtHandler.ReadJwtToken(tokenStore.Tokens.AccessToken);
            if (current.ValidTo >= DateTime.UtcNow)
            {
                return true;
            }
        }

        if (tokenStore.Tokens?.RefreshToken is null)
        {
            return false;
        }

        try
        {
            var refreshed = await authenticationService.RefreshAsync(tokenStore.Tokens.RefreshToken);
            if (refreshed is not null)
            {
                await PersistTokensAsync(refreshed);
                return true;
            }
        }
        catch
        {
            // Any failure = session expired
        }

        await ClearTokensAsync();
        return false;
    }

    private async Task PersistTokensAsync(AccessTokensDto tokens)
    {
        tokenStore.Tokens = tokens;
        await localStorage.SetAsync(AuthCacheKeys.TokenKey, tokens);
    }

    private async Task ClearTokensAsync()
    {
        tokenStore.Tokens = null;
        await localStorage.DeleteAsync(AuthCacheKeys.TokenKey);
    }

    private static AuthenticationState Anonymous() =>
        new(new ClaimsPrincipal(new ClaimsIdentity()));
}
