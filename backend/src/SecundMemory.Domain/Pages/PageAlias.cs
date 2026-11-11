using SecundMemory.Domain.Common;
using SecundMemory.Domain.Exceptions;

namespace SecundMemory.Domain.Pages;

public sealed class PageAlias : ValueObject
{
    public string Value { get; }

    private PageAlias(string value) => Value = value;

    public static PageAlias Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BusinessRuleException("PageAliasRequired", "Page alias cannot be empty.");

        return new PageAlias(value.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
