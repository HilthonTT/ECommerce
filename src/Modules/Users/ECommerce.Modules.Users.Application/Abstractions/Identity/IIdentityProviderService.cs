using ECommerce.Common.Domain;

namespace ECommerce.Modules.Users.Application.Abstractions.Identity;

public interface IIdentityProviderService
{
    Task<Result<AccessTokensResponse>> LoginUserAsync(LoginModel login, CancellationToken cancellationToken = default);

    Task<Result<AccessTokensResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<Result<string>> RegisterUserAsync(UserModel user, CancellationToken cancellationToken = default);
}
