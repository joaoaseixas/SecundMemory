namespace SecundMemory.Domain.Exceptions;

/// <summary>
/// An invariant of the domain model itself was violated — reserved for
/// programming errors (calling code didn't respect a precondition), not for
/// expected business outcomes (use Result for those).
/// </summary>
public class DomainException(string message) : Exception(message);
