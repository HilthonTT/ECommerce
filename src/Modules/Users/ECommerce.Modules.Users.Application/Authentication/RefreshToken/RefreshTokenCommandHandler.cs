using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Application.Abstractions.Identity;

namespace ECommerce.Modules.Users.Application.Authentication.RefreshToken;

internal sealed class RefreshTokenCommandHandler(IIdentityProviderService identityProviderService)
    : ICommandHandler<RefreshTokenCommand, AccessTokensResponse>
{
    public async Task<Result<AccessTokensResponse>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        Result<AccessTokensResponse> result = await identityProviderService.RefreshTokenAsync(
            command.RefreshToken,
            cancellationToken);

        if (result.IsFailure)
        {
            return result.Error;
        }

        return result;
    }
}
