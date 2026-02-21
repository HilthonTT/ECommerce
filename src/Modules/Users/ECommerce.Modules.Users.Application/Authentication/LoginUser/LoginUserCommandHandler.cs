using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Application.Abstractions.Identity;

namespace ECommerce.Modules.Users.Application.Authentication.LoginUser;

internal sealed class LoginUserCommandHandler(IIdentityProviderService identityProviderService)
    : ICommandHandler<LoginUserCommand, AccessTokensResponse>
{
    public async Task<Result<AccessTokensResponse>> Handle(
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

        return result;
    }
}
