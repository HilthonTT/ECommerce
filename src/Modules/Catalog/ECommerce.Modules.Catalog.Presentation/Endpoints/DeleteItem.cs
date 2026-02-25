using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Catalog.Application.Catalog.DeleteItem;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Catalog.Presentation.Endpoints;

internal sealed class DeleteItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("items/{id:int}", async (
            int id,
            ICommandHandler<DeleteItemCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteItemCommand(id);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, ApiResults.Problem);
        })
        .WithTags(Tags.Catalog)
        .RequireAuthorization(Permissions.RemoveItem);
    }
}