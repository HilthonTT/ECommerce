using ECommerce.Common.Application.Authorization;
using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Users.Application.Users.GetUserPermissions;

public sealed record GetUserPermissionsQuery(string IdentityId) : IQuery<PermissionResponse>;
