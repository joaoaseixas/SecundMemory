using System.Text.RegularExpressions;

namespace SecundMemory.Domain.Links;

/// <summary>
/// Pure parsing rules for what counts as a link inside block content — no
/// I/O, no persistence, so it needs no interface/DI to be testable.
/// </summary>
public static partial class LinkParser
{
    /// <summary>Extracts the page titles referenced via [[Page Title]].</summary>
    public static IReadOnlyList<string> ExtractPageLinks(string content)
    {
        if (string.IsNullOrEmpty(content)) return [];

        return PageLinkRegex()
            .Matches(content)
            .Select(m => m.Groups[1].Value.Trim())
            .Where(title => title.Length > 0)
            .Distinct()
            .ToList();
    }

    /// <summary>Extracts the block ids referenced via ((block-id)).</summary>
    public static IReadOnlyList<Guid> ExtractBlockReferences(string content)
    {
        if (string.IsNullOrEmpty(content)) return [];

        return BlockEmbedRegex()
            .Matches(content)
            .Select(m => Guid.TryParse(m.Groups[1].Value, out var id) ? id : (Guid?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();
    }

    [GeneratedRegex(@"\[\[([^\[\]]+)\]\]")]
    private static partial Regex PageLinkRegex();

    [GeneratedRegex(@"\(\(([0-9a-fA-F-]{36})\)\)")]
    private static partial Regex BlockEmbedRegex();
}
