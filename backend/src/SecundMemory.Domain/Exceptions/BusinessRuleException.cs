namespace SecundMemory.Domain.Exceptions;

public class BusinessRuleException(string ruleName, string message)
    : DomainException($"Business rule '{ruleName}' was violated: {message}")
{
    public string RuleName { get; } = ruleName;
}
