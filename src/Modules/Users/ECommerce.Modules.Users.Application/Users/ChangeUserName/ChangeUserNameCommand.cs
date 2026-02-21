using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Users.Application.Users.ChangeUserName;

public sealed record ChangeUserNameCommand(Guid UserId, string FirstName, string LastName) : ICommand<UserResponse>;
