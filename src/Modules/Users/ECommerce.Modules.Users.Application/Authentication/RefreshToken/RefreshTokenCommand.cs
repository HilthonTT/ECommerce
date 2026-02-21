using ECommerce.Common.Application.Messaging;
using ECommerce.Modules.Users.Application.Abstractions.Identity;

namespace ECommerce.Modules.Users.Application.Authentication.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AccessTokensResponse>;
