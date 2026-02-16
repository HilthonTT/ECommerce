namespace ECommerce.Webhooks.Application.Abstractions.Authentication;

public interface IUserContext
{
    Guid UserId { get; }
}
