using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Catalog.Application.Catalog.UpdateItem;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Catalog.Presentation.Endpoints;

internal sealed class UpdateItem : IEndpoint
{
    private sealed record Request(string Name, string? Description, decimal Price);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("catalog/items/{id:int}", async (
            int id,
            Request request,
            ICommandHandler<UpdateItemCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateItemCommand(id, request.Name, request.Description, request.Price);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .WithTags(Tags.Catalog)
        .RequireAuthorization(Permissions.UpdateItem);
    }
}
