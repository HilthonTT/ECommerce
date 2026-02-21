using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Users.Application.Users.GetUser;

public sealed record GetUserQuery(Guid UserId) : IQuery<UserResponse>;
