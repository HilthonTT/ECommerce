using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Users.Application.Abstractions.Identity;
using ECommerce.Modules.Users.Application.TwoFactor.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Users.Presentation.TwoFactor;

internal sealed class Setup : IEndpoint
{
    private sealed record Request(string Email);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("2fa/setup", async (
            Request request,
            IUserContext userContext,
            ICommandHandler<SetupTwoFactorCommand, SetupTwoFactorResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new SetupTwoFactorCommand(userContext.UserId, request.Email), cancellationToken);
            return result.Match((res) => Results.File(res.QrCodeImage, "image/png"), ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.TwoFactor);
    }
}
