using ECommerce.Common.Application.Messaging;

namespace ECommerce.Modules.Users.Application.Authentication.LoginUser;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<LoginResponse>;
