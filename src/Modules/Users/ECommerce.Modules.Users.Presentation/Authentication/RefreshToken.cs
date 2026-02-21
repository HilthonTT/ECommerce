using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Users.Application.Abstractions.Identity;
using ECommerce.Modules.Users.Application.Authentication.RefreshToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Users.Presentation.Authentication;

internal sealed class RefreshToken : IEndpoint
{
    private sealed record Request(string RefreshToken);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/refresh", async (
            Request request,
            ICommandHandler<RefreshTokenCommand, AccessTokensResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<AccessTokensResponse> result = await handler.Handle(
                new RefreshTokenCommand(request.RefreshToken),
                cancellationToken);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Authentication)
        .WithName(nameof(RefreshToken))
        .WithTags(Tags.Authentication);
    }
}
