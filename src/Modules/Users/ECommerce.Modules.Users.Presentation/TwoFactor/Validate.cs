using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Users.Application.Abstractions.Identity;
using ECommerce.Modules.Users.Application.TwoFactor.Validate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Users.Presentation.TwoFactor;

internal sealed class Validate : IEndpoint
{
    private sealed record Request(string Code, string LimitedToken);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("2fa/validate", async (
            Request request,
            IUserContext userContext,
            ICommandHandler<ValidateTwoFactorCommand, ValidateTwoFactorResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(
                new ValidateTwoFactorCommand(userContext.UserId, request.Code, request.LimitedToken),
                cancellationToken);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.TwoFactor);
    }
}
