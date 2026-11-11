using SecundMemory.Domain.Common;

namespace SecundMemory.Domain.Links;

public enum BlockReferenceType
{
    /// <summary>A [[Page Title]] link found inside a block's content.</summary>
    PageLink,

    /// <summary>A ((block-id)) embed found inside a block's content.</summary>
    BlockEmbed,
}

/// <summary>
/// A derived/indexed fact, not something created directly by a user command —
/// it is written by the link indexer whenever a block's content changes (see
/// Infrastructure.Search.SearchIndexUpdater), by re-parsing the content with
/// LinkParser. Backlinks on a Page are just "BlockReferences where
/// TargetPageId = this page", read straight off this table.
/// </summary>
public class BlockReference : Entity
{
    public Guid SourceBlockId { get; private init; }
    public Guid? TargetPageId { get; private init; }
    public Guid? TargetBlockId { get; private init; }
    public BlockReferenceType Type { get; private init; }
    public DateTime CreatedAtUtc { get; private init; }

    private BlockReference() { }

    public static BlockReference ToPage(Guid sourceBlockId, Guid targetPageId) => new()
    {
        SourceBlockId = sourceBlockId,
        TargetPageId = targetPageId,
        Type = BlockReferenceType.PageLink,
        CreatedAtUtc = DateTime.UtcNow,
    };

    public static BlockReference ToBlock(Guid sourceBlockId, Guid targetBlockId) => new()
    {
        SourceBlockId = sourceBlockId,
        TargetBlockId = targetBlockId,
        Type = BlockReferenceType.BlockEmbed,
        CreatedAtUtc = DateTime.UtcNow,
    };
}
