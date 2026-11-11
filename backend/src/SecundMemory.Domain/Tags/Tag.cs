using SecundMemory.Domain.Common;
using SecundMemory.Domain.Properties;

namespace SecundMemory.Domain.Tags;

/// <summary>
/// A template for a property a tagged block is expected to carry — e.g. the
/// "book" tag might template a "status" Text property defaulting to
/// "reading". Applying a Tag to a Block (Application layer) seeds matching
/// BlockProperty rows from these templates; this is how "tags act as
/// classes" without Tag and Block being the same aggregate.
/// </summary>
public sealed class TagPropertyTemplate : ValueObject
{
    public string Key { get; }
    public PropertyType Type { get; }
    public string? DefaultValue { get; }
    public bool AllowMultiple { get; }

    public TagPropertyTemplate(string key, PropertyType type, string? defaultValue, bool allowMultiple)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Property template key cannot be empty.", nameof(key));

        Key = key.Trim();
        Type = type;
        DefaultValue = defaultValue;
        AllowMultiple = allowMultiple;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Key;
        yield return Type;
        yield return DefaultValue;
        yield return AllowMultiple;
    }
}

public class Tag : AggregateRoot
{
    private readonly List<TagPropertyTemplate> _propertyTemplates = [];

    public Guid WorkspaceId { get; private init; }
    public string Name { get; private set; } = string.Empty;
    public IReadOnlyCollection<TagPropertyTemplate> PropertyTemplates => _propertyTemplates.AsReadOnly();

    private Tag() { }

    public static Tag Create(Guid workspaceId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name cannot be empty.", nameof(name));

        return new Tag { WorkspaceId = workspaceId, Name = name.Trim() };
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name cannot be empty.", nameof(name));

        Name = name.Trim();
    }

    public void AddPropertyTemplate(TagPropertyTemplate template)
    {
        if (_propertyTemplates.Any(t => t.Key.Equals(template.Key, StringComparison.OrdinalIgnoreCase)))
        {
            _propertyTemplates.RemoveAll(t => t.Key.Equals(template.Key, StringComparison.OrdinalIgnoreCase));
        }

        _propertyTemplates.Add(template);
    }

    public void RemovePropertyTemplate(string key) =>
        _propertyTemplates.RemoveAll(t => t.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
}
