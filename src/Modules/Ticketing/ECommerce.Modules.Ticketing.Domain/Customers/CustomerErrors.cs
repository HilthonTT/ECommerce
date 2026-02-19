using ECommerce.Common.Domain;

namespace ECommerce.Modules.Ticketing.Domain.Customers;

public static class CustomerErrors
{
    public static readonly Error Unauthorized = Error.Authorization(
        "Customer.Unauthorized",
        "You are not authorized to perform this action.");

    public static Error NotFound(Guid customerId) =>
        Error.NotFound("Customers.NotFound", $"The customer with the identifier {customerId} not found.");

    public static readonly Error CustomerIdCannotBeEmpty = Error.Problem(
       "Customer.CustomerIdCannotBeEmpty",
       "Customer ID cannot be empty.");

    public static readonly Error EmailIsRequired = Error.Problem(
        "Customer.EmailIsRequired",
        "Email is required.");

    public static readonly Error FirstNameIsRequired = Error.Problem(
        "Customer.FirstNameIsRequired",
        "First name is required.");

    public static readonly Error LastNameIsRequired = Error.Problem(
        "Customer.LastNameIsRequired",
        "Last name is required.");

    public static readonly Error PaymentMethodNotFound = Error.Problem(
        "Customer.PaymentMethodNotFound",
        "Payment method not found.");
}