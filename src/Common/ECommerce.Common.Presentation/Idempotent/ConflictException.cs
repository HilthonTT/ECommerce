namespace ECommerce.Common.Presentation.Idempotent;

/// <summary>
/// Thrown when a duplicate request with the same Idempotence-Key
/// is received while the original is still being processed.
/// Handle this in your global exception handler to return 409 Conflict.
/// </summary>
public sealed class ConflictException(string message) : Exception(message);
