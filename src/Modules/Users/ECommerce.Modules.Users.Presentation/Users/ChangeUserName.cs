using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Users.Application.Users;
using ECommerce.Modules.Users.Application.Users.ChangeUserName;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Users.Presentation.Users;

internal sealed class ChangeUserName : IEndpoint
{
    private sealed record Request(string FirstName, string LastName);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/{id:guid}/profile", async (
            Guid id,
            Request request,
            ICommandHandler<ChangeUserNameCommand, UserResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(
                new ChangeUserNameCommand(id, request.FirstName, request.LastName),
                cancellationToken);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization(Permissions.ChangeUserName)
        .WithTags(Tags.Users);
    }
}
