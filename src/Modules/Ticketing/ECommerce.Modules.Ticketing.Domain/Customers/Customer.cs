using ECommerce.Common.Domain;
using ECommerce.Common.Domain.Auditing;

namespace ECommerce.Modules.Ticketing.Domain.Customers;

[Auditable]
public sealed class Customer : Entity
{
    private readonly List<PaymentMethod> _paymentMethods = [];

    public Guid Id { get; private init; }

    public string Email { get; private set; } = string.Empty;

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public IReadOnlyCollection<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();

    private Customer() { }

    public static Customer Create(Guid id, string email, string firstName, string lastName)
    {
        var customer = new Customer
        {
            Id = id,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        customer.RaiseDomainEvent(new CustomerCreatedDomainEvent(customer.Id));

        return customer;
    }

    public void ChangeName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;

        RaiseDomainEvent(new CustomerNameChangedDomainEvent(Id, FirstName, LastName));
    }

    public Result<PaymentMethod> VerifyOrAddPaymentMethod(
       int cardTypeId,
       string alias,
       string cardNumber,
       string securityNumber,
       string cardHolderName,
       DateTime expiration,
       Guid orderId)
    {
        // Check if payment method already exists
        PaymentMethod? existingPaymentMethod = _paymentMethods
            .SingleOrDefault(pm => pm.IsEqualTo(cardTypeId, cardNumber, expiration));

        if (existingPaymentMethod is not null)
        {
            RaiseDomainEvent(new PaymentMethodVerifiedDomainEvent(
                Id,
                existingPaymentMethod.Id,
                orderId));

            return Result.Success(existingPaymentMethod);
        }

        // Create new payment method
        Result<PaymentMethod> paymentMethodResult = PaymentMethod.Create(
            Id,
            cardTypeId,
            alias,
            cardNumber,
            securityNumber,
            cardHolderName,
            expiration);

        if (paymentMethodResult.IsFailure)
        {
            return paymentMethodResult;
        }

        _paymentMethods.Add(paymentMethodResult.Value);

        RaiseDomainEvent(new PaymentMethodAddedToCustomerDomainEvent(
            Id,
            paymentMethodResult.Value.Id,
            orderId));

        return Result.Success(paymentMethodResult.Value);
    }

    public Result<PaymentMethod> GetPaymentMethod(Guid paymentMethodId)
    {
        PaymentMethod? paymentMethod = _paymentMethods
            .SingleOrDefault(pm => pm.Id == paymentMethodId);

        if (paymentMethod is null)
        {
            return Result.Failure<PaymentMethod>(CustomerErrors.PaymentMethodNotFound);
        }

        return Result.Success(paymentMethod);
    }

    public Result RemovePaymentMethod(Guid paymentMethodId)
    {
        PaymentMethod? paymentMethod = _paymentMethods
            .SingleOrDefault(pm => pm.Id == paymentMethodId);

        if (paymentMethod is null)
        {
            return CustomerErrors.PaymentMethodNotFound;
        }

        _paymentMethods.Remove(paymentMethod);

        RaiseDomainEvent(new PaymentMethodRemovedDomainEvent(Id, paymentMethodId));

        return Result.Success();
    }
}
