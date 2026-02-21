using Dapper;
using ECommerce.Common.Application.Data;
using ECommerce.Common.Application.Messaging;
using ECommerce.Common.Domain;
using ECommerce.Modules.Users.Domain.Users;

namespace ECommerce.Modules.Users.Application.Users.GetUser;

internal sealed class GetUserQueryHandler(IDbConnectionFactory dbConnectionFactory) 
    : IQueryHandler<GetUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        await using var dbConnection = await dbConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            $"""
            SELECT
               id as {nameof(UserResponse.Id)},
               email as {nameof(UserResponse.Email)},
               first_name as {nameof(UserResponse.FirstName)},
               last_name as {nameof(UserResponse.LastName)}
            FROM users.users
            WHERE id = @UserId
            """;

        UserResponse? user = await dbConnection.QueryFirstOrDefaultAsync<UserResponse>(sql, query);

        if (user is null)
        {
            return UserErrors.NotFound(query.UserId);
        }

        return user;
    }
}
