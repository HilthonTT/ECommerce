using ECommerce.Web.Shared.DTOs.Authentication;
using ECommerce.Web.Shared.Services.Authentication.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public sealed class TokenAuthStateProvider(
    IAuthenticationService authenticationService,
    ProtectedLocalStorage localStorage) : AuthenticationStateProvider
{
    private const string TokenKey = "auth_tokens";

    private AccessTokensDto? _tokens;
    private bool _initialized;
    private Task<bool>? _refreshTask;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!_initialized)
        {
            _initialized = true;
            try
            {
                var result = await localStorage.GetAsync<AccessTokensDto>(TokenKey);
                if (result.Success && result.Value is not null)
                {
                    _tokens = result.Value;
                }
            }
            catch (InvalidOperationException)
            {
                return Anonymous();
            }
        }

        if (_tokens is null)
            return Anonymous();

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(_tokens.AccessToken);

        if (jwt.ValidTo < DateTime.UtcNow)
        {
            // Coalesce concurrent refresh attempts into one Task
            _refreshTask ??= TryRefreshAsync();
            var refreshed = await _refreshTask;
            _refreshTask = null;

            if (!refreshed)
                return Anonymous();

            jwt = handler.ReadJwtToken(_tokens!.AccessToken);
        }

        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task<LoginDto?> LoginAsync(string email, string password)
    {
        var result = await authenticationService.LoginAsync(email, password);

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
            var refreshed = await authenticationService.RefreshAsync(_tokens.RefreshToken);
            if (refreshed is not null)
            {
                _tokens = refreshed;
                await localStorage.SetAsync(TokenKey, _tokens);
                return true;
            }
        }
        catch (Exception)  // catch-all — GetAuthenticationStateAsync must never throw
        {
            // Any failure (HttpRequestException, InvalidOperationException, etc.)
            // is treated as "session expired"
        }

        _tokens = null;
        await localStorage.DeleteAsync(TokenKey);
        return false;
    }

    private static AuthenticationState Anonymous() =>
        new(new ClaimsPrincipal(new ClaimsIdentity()));
}
