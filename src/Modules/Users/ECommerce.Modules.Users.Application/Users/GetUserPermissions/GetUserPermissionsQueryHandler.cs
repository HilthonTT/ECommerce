using Dapper;
using ECommerce.Common.Application.Authorization;
using ECommerce.Common.Application.Data;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Domain.Users;

namespace ECommerce.Modules.Users.Application.Users.GetUserPermissions;

internal sealed class GetUserPermissionsQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetUserPermissionsQuery, PermissionResponse>
{
    public async Task<Result<PermissionResponse>> Handle(GetUserPermissionsQuery query, CancellationToken cancellationToken)
    {
        await using var connection = await dbConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            $"""
             SELECT DISTINCT
                u.id AS {nameof(UserPermission.UserId)},
                rp.permission_code AS {nameof(UserPermission.Permission)}
             FROM users.users u
             JOIN users.user_roles ur ON u.id = ur.user_id
             JOIN users.role_permissions rp ON ur.role_name = rp.role_name
             WHERE u.identity_id = @IdentityId
             """;

        List<UserPermission> permissions = (await connection.QueryAsync<UserPermission>(sql, query)).ToList();
        if (permissions.Count == 0)
        {
            return UserErrors.NotFound(query.IdentityId);
        }

        return new PermissionResponse(permissions[0].UserId,
            permissions.Select(userPermission => userPermission.Permission).ToHashSet());
    }

    internal sealed class UserPermission
    {
        internal Guid UserId { get; init; }

        internal string Permission { get; init; } = string.Empty;
    }
}
