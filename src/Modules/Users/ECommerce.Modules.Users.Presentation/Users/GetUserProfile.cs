using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Users.Application.Abstractions.Identity;
using ECommerce.Modules.Users.Application.Users;
using ECommerce.Modules.Users.Application.Users.GetUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Users.Presentation.Users;

internal sealed class GetUserProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users", async (
            IUserContext userContext,
            IQueryHandler<GetUserQuery, UserResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserQuery(userContext.UserId);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization(Permissions.GetUserProfile)
        .WithTags(Tags.Users);
    }
}
