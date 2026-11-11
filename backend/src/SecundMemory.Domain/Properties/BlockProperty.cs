using SecundMemory.Domain.Common;

namespace SecundMemory.Domain.Properties;

public enum PropertyType
{
    Text,
    Number,
    Date,
    DateTime,
    Checkbox,
    Url,

    /// <summary>A reference to another Page or Block, stored as that entity's Guid.</summary>
    Node,
}

/// <summary>
/// A single typed property value on a Block. Fuses what the original design
/// split into PropertyDefinition+PropertyValue — for this project's size, one
/// entity per (block, key) is simpler, and Tag.PropertyTemplates already
/// plays the "shared schema" role when a tag seeds these on a block.
/// </summary>
public class BlockProperty : Entity
{
    public Guid BlockId { get; private init; }
    public string Key { get; private init; } = string.Empty;
    public PropertyType Type { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private init; }
    public DateTime UpdatedAtUtc { get; private set; }

    private BlockProperty() { }

    public static BlockProperty Create(Guid blockId, string key, PropertyType type, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Property key cannot be empty.", nameof(key));

        var now = DateTime.UtcNow;
        return new BlockProperty
        {
            BlockId = blockId,
            Key = key.Trim(),
            Type = type,
            Value = value ?? string.Empty,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
    }

    public void UpdateValue(string value)
    {
        Value = value ?? string.Empty;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
