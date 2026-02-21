using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Users.Application.Users;

namespace ECommerce.Modules.Users.Application.Authentication.RegisterUser;

public sealed record RegisterUserCommand(string Email, string Password, string ConfirmPassword, string FirstName, string LastName)
    : ICommand<UserResponse>;
