using ECommerce.Common.Application.Authorization;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Application.Users.GetUserPermissions;

namespace ECommerce.Modules.Users.Infrastructure.Authorization;

internal sealed class PermissionService(IQueryHandler<GetUserPermissionsQuery, PermissionResponse> handler)
    : IPermissionService
{
    public Task<Result<PermissionResponse>> GetUserPermissionsAsync(string identityId)
    {
        return handler.Handle(new GetUserPermissionsQuery(identityId), CancellationToken.None);
    }
}
