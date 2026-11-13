using SecundMemory.Domain.Common;

namespace SecundMemory.Domain.Blocks.Events;

public sealed class BlockCreatedEvent(Guid blockId, Guid pageId, string content = "") : IDomainEvent
{
    public Guid BlockId { get; } = blockId;
    public Guid PageId { get; } = pageId;
    public string Content { get; } = content;
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
