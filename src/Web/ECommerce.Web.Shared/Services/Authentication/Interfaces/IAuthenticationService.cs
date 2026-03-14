using ECommerce.Web.Shared.DTOs.Authentication;

namespace ECommerce.Web.Shared.Services.Authentication.Interfaces;

public interface IAuthenticationService
{
    Task<LoginDto?> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<UserDto?> RegisterAsync(
        string email, 
        string password, 
        string confirmPassword,
        string firstName, 
        string lastName,
        CancellationToken cancellationToken = default);

    Task<AccessTokensDto?> RefreshAsync(
        string refreshToken, 
        CancellationToken cancellationToken = default);
}
