using Microsoft.AspNetCore.Routing;

namespace ECommerce.Common.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
