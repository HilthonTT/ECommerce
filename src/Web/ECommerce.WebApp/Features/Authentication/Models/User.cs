namespace ECommerce.WebApp.Features.Authentication.Models;

public sealed record User(Guid Id, string Email, string FirstName, string LastName);
