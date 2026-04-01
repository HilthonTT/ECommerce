using ECommerce.Common.Application.DTOs;
using ECommerce.Common.Application.Links;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ECommerce.Common.Infrastructure.Links;

internal sealed class LinkService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor) 
    : ILinkService
{
    private HttpContext HttpContext =>
        httpContextAccessor.HttpContext
        ?? throw new InvalidOperationException("Cannot generate links outside of an active HTTP request context.");

    public LinkDto CreateForAction(string action, string rel, string method, object? values = null, string? controller = null)
    {
        string href = linkGenerator.GetUriByAction(
            HttpContext,
            action,
            controller,
            values) 
            ?? throw new InvalidOperationException($"Could not generate URI for action '{action}' on controller '{controller ?? "current"}'.");
    
        return LinkDto.Create(href, rel, method);
    }

    public LinkDto CreateForEndpoint(string endpointName, string rel, string method, object? values = null)
    {
        string href = linkGenerator.GetUriByName(HttpContext, endpointName, values)
            ?? throw new InvalidOperationException($"Could not generate URI for endpoint '{endpointName}'. Ensure it has been registered with .WithName(\"{endpointName}\").");

        return LinkDto.Create(href, rel, method); throw new NotImplementedException();
    }
}
