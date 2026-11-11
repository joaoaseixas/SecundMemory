using SecundMemory.Domain.Common;

namespace SecundMemory.Domain.Blocks.Events;

/// <summary>
/// Raised whenever a block's content changes — the trigger for the link
/// indexer (Infrastructure.Search.SearchIndexUpdater) to re-parse
/// [[page links]] and ((block refs)) out of the new content.
/// </summary>
public sealed class BlockContentUpdatedEvent(Guid blockId, Guid pageId, string newContent) : IDomainEvent
{
    public Guid BlockId { get; } = blockId;
    public Guid PageId { get; } = pageId;
    public string NewContent { get; } = newContent;
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
