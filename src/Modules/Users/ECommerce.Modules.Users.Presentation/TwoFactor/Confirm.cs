using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Users.Application.Abstractions.Identity;
using ECommerce.Modules.Users.Application.TwoFactor.ConfirmTwoFactor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Users.Presentation.TwoFactor;

internal sealed class Confirm : IEndpoint
{
    private sealed record Request(string Code);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("2fa/confirm", async (
            Request request,
            IUserContext userContext,
            ICommandHandler<ConfirmTwoFactorCommand, ConfirmTwoFactorResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new ConfirmTwoFactorCommand(userContext.UserId, request.Code), cancellationToken);
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.TwoFactor);
    }
}
