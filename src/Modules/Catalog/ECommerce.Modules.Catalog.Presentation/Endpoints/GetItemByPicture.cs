using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Presentation.ApiResults;
using ECommerce.Common.Presentation.Endpoints;
using ECommerce.Modules.Catalog.Application.Catalog.GetItemPicture;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Modules.Catalog.Presentation.Endpoints;

internal sealed class GetItemByPicture : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("items/{id:int}/picture", async (
            int id,
            IQueryHandler<GetItemPictureQuery, PhysicalFileResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetItemPictureQuery(id), cancellationToken);
            return result.Match(
                (res) => TypedResults.PhysicalFile(res.Path, res.MimeType, lastModified: res.LastModified), 
                ApiResults.Problem);
        })
        .WithTags(Tags.Catalog);
    }
}
