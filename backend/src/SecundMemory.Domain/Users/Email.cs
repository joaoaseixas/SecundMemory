using System.Text.RegularExpressions;
using SecundMemory.Domain.Common;
using SecundMemory.Domain.Exceptions;

namespace SecundMemory.Domain.Users;

public sealed partial class Email : ValueObject
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BusinessRuleException("EmailRequired", "Email cannot be empty.");

        value = value.Trim();

        if (!EmailRegex().IsMatch(value))
            throw new BusinessRuleException("EmailInvalid", $"'{value}' is not a valid email address.");

        return new Email(value.ToLowerInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
