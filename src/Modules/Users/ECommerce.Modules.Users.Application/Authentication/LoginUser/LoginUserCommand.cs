using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Users.Application.Abstractions.Identity;

namespace ECommerce.Modules.Users.Application.Authentication.LoginUser;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<AccessTokensResponse>;
