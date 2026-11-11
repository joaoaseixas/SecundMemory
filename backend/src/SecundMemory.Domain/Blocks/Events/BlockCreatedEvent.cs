using SecundMemory.Domain.Common;

namespace SecundMemory.Domain.Blocks.Events;

public sealed class BlockCreatedEvent(Guid blockId, Guid pageId) : IDomainEvent
{
    public Guid BlockId { get; } = blockId;
    public Guid PageId { get; } = pageId;
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
