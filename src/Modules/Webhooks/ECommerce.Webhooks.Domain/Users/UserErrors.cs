using ECommerce.Common.Domain;

namespace ECommerce.Webhooks.Domain.Users;

public static class UserErrors
{
    public static readonly Error Unauthorized = Error.Authorization(
        "User.Unauthorized",
        "You are not authorized to perform this action.");
}
