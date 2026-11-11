using SecundMemory.Domain.Common;

namespace SecundMemory.Domain.Blocks.Events;

public sealed class BlockMovedEvent(Guid blockId, Guid? newParentBlockId, double newOrder) : IDomainEvent
{
    public Guid BlockId { get; } = blockId;
    public Guid? NewParentBlockId { get; } = newParentBlockId;
    public double NewOrder { get; } = newOrder;
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
