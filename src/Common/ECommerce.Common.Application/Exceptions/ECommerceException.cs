using ECommerce.Common.Domain;

namespace ECommerce.Common.Application.Exceptions;

public sealed class ECommerceException(
    string requestName,
    Error? error = default,
    Exception? innerException = default)
    : Exception("Application exception", innerException)
{
    public string RequestName { get; } = requestName;

    public Error? Error { get; } = error;
}
