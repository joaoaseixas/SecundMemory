using SecundMemory.Domain.Common;
using SecundMemory.Domain.Exceptions;

namespace SecundMemory.Domain.Blocks;

public sealed class BlockContent : ValueObject
{
    public const int MaxLength = 32_768;

    public string Value { get; }

    private BlockContent(string value) => Value = value;

    public static BlockContent Create(string value)
    {
        value ??= string.Empty;

        if (value.Length > MaxLength)
            throw new BusinessRuleException("BlockContentTooLong", $"Block content cannot exceed {MaxLength} characters.");

        return new BlockContent(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
