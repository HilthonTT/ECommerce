using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Users.Application.Authentication.RegisterUser;
using ECommerce.Modules.Users.Application.Users;
using ECommerce.Modules.Users.Presentation.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Users.Presentation.Authentication;

internal sealed class RegisterUser : IEndpoint
{
    private sealed record Request(
        string Email, 
        string Password, 
        string ConfirmPassword, 
        string FirstName, 
        string LastName);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/register", async (
            Request request,
            ICommandHandler<RegisterUserCommand, UserResponse> handler,
            HttpContext context,
            LinkGenerator linkGenerator,
            CancellationToken cancellationToken) =>
        {
            Result<UserResponse> result = await handler.Handle(new RegisterUserCommand(
                    request.Email,
                    request.Password,
                    request.ConfirmPassword,
                    request.FirstName,
                    request.LastName),
                    cancellationToken);

            return result.Match(
                () => Results.Created(
                    linkGenerator.GetUriByName(context, nameof(GetUserProfile), new { id = result.Value.Id }),
                    result.Value),
                ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithName(nameof(RegisterUser))
        .WithTags(Tags.Authentication);
    }
}
