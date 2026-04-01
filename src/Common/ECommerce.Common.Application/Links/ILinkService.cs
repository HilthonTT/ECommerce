using ECommerce.Common.Application.DTOs;

namespace ECommerce.Common.Application.Links;

public interface ILinkService
{
    LinkDto CreateForAction(string action, string rel, string method, object? values = null, string? controller = null);

    LinkDto CreateForEndpoint(string endpointName, string rel, string method, object? values = null);
}
