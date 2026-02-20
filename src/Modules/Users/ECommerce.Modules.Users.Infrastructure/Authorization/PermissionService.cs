using ECommerce.Common.Application.Authorization;
using ECommerce.Common.Domain;

namespace ECommerce.Modules.Users.Infrastructure.Authorization;

internal sealed class PermissionService()
    : IPermissionService
{
    public async Task<Result<PermissionResponse>> GetUserPermissionsAsync(string identityId)
    {
        // TODO: Implement this later
        throw new NotImplementedException();
    }
}