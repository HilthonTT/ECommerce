using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Application.Abstractions.Identity;
using ECommerce.Modules.Users.Domain.Users;

namespace ECommerce.Modules.Users.Application.Authentication.LoginUser;

internal sealed class LoginUserCommandHandler(
    IIdentityProviderService identityProviderService, 
    IUserRepository userRepository,
    ITokenService tokenService)
    : ICommandHandler<LoginUserCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        Result<AccessTokensResponse> result = await identityProviderService.LoginUserAsync(
            new LoginModel(
                command.Email,
                command.Password),
            cancellationToken);

        if (result.IsFailure)
        {
            return result.Error;
        }

        User? user = await userRepository.GetByEmailAsync(command.Email, cancellationToken);

        if (user is { TwoFactorEnabled: true })
        {
            string limitedToken = tokenService.GenerateLimitedToken(user.Id, purpose: "2fa");

            return new LoginResponse(
                RequiresTwoFactor: true,
                LimitedToken: limitedToken,
                AccessTokens: null);
        }

        return new LoginResponse(
            RequiresTwoFactor: false,
            LimitedToken: null,
            AccessTokens: result.Value);
    }
}
