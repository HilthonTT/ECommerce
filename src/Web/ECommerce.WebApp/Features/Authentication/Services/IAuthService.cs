using ECommerce.WebApp.Features.Authentication.Models;

namespace ECommerce.WebApp.Features.Authentication.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(string email, string password);

    Task<User?> RegisterAsync(
        string email, string password, string confirmPassword,
        string firstName, string lastName);

    Task<AccessTokensResponse?> RefreshAsync(string refreshToken);
}
