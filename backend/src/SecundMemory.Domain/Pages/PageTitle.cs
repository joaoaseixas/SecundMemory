using SecundMemory.Domain.Common;
using SecundMemory.Domain.Exceptions;

namespace SecundMemory.Domain.Pages;

public sealed class PageTitle : ValueObject
{
    public const int MaxLength = 256;

    public string Value { get; }

    private PageTitle(string value) => Value = value;

    public static PageTitle Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BusinessRuleException("PageTitleRequired", "Page title cannot be empty.");

        value = value.Trim();

        if (value.Length > MaxLength)
            throw new BusinessRuleException("PageTitleTooLong", $"Page title cannot exceed {MaxLength} characters.");

        return new PageTitle(value);
    }

    /// <summary>
    /// The namespace portion of a "projeto/subprojeto" style title — everything
    /// before the last '/', or null if the title has no namespace segment.
    /// </summary>
    public string? Namespace
    {
        get
        {
            var lastSlash = Value.LastIndexOf('/');
            return lastSlash <= 0 ? null : Value[..lastSlash];
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
