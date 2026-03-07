using ECommerce.WebApp.Features.Authentication.Models;
using ECommerce.WebApp.Features.Authentication.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.WebApp.Features.Authentication.Providers;

internal sealed class TokenAuthStateProvider(
    IAuthService authService,
    ProtectedLocalStorage localStorage) : AuthenticationStateProvider
{
    private const string TokenKey = "auth_tokens";

    private AccessTokensResponse? _tokens;
    private bool _initialized;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!_initialized)
        {
            _initialized = true;
            try
            {
                ProtectedBrowserStorageResult<AccessTokensResponse> result = 
                    await localStorage.GetAsync<AccessTokensResponse>(TokenKey);

                if (result.Success && result.Value is not null)
                {
                    _tokens = result.Value;
                }
            }
            catch (InvalidOperationException)
            {
                // JS interop not available during prerender — return anonymous
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        if (_tokens is null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(_tokens.AccessToken);

        if (jwt.ValidTo < DateTime.UtcNow)
        {
            // Token expired — try refresh
            var refreshed = await TryRefreshAsync();
            if (!refreshed)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            jwt = handler.ReadJwtToken(_tokens!.AccessToken);
        }

        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var result = await authService.LoginAsync(email, password);

        if (result?.AccessTokens is not null)
        {
            _tokens = result.AccessTokens;
            await localStorage.SetAsync(TokenKey, _tokens);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        return result;
    }

    public async Task LogoutAsync()
    {
        _tokens = null;
        await localStorage.DeleteAsync(TokenKey);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private async Task<bool> TryRefreshAsync()
    {
        if (_tokens?.RefreshToken is null)
        {
            return false;
        }

        try
        {
            var refreshed = await authService.RefreshAsync(_tokens.RefreshToken);
            if (refreshed is not null)
            {
                _tokens = refreshed;
                await localStorage.SetAsync(TokenKey, _tokens);
                return true;
            }
        }
        catch (HttpRequestException)
        {
            // Refresh failed
        }

        _tokens = null;
        await localStorage.DeleteAsync(TokenKey);
        return false;
    }
}
