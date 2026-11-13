using SecundMemory.Domain.Blocks.Events;
using SecundMemory.Domain.Common;

namespace SecundMemory.Domain.Blocks;

/// <summary>
/// Its own aggregate root rather than a child of Page — an outliner edits,
/// moves, and collapses individual blocks constantly, and loading/saving an
/// entire page's block tree on every keystroke would not scale. Pages and
/// Blocks are related only by Id (PageId), never by object reference.
/// </summary>
public class Block : AggregateRoot, ISoftDeletable
{
    private readonly List<Guid> _tagIds = [];

    public Guid PageId { get; private init; }
    public Guid? ParentBlockId { get; private set; }
    public BlockContent Content { get; private set; } = null!;
    public BlockType Type { get; private set; }
    public double Order { get; private set; }
    public bool IsCollapsed { get; private set; }
    public IReadOnlyCollection<Guid> TagIds => _tagIds.AsReadOnly();

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    private Block() { }

    public static Block Create(Guid pageId, Guid? parentBlockId, BlockContent content, double order, BlockType type = BlockType.Text)
    {
        var block = new Block
        {
            PageId = pageId,
            ParentBlockId = parentBlockId,
            Content = content,
            Type = type,
            Order = order,
        };

        block.RaiseDomainEvent(new BlockCreatedEvent(block.Id, pageId, content.Value));
        return block;
    }

    public void UpdateContent(BlockContent content)
    {
        if (content.Value == Content.Value) return;

        Content = content;
        RaiseDomainEvent(new BlockContentUpdatedEvent(Id, PageId, content.Value));
    }

    public void Move(Guid? newParentBlockId, double newOrder)
    {
        ParentBlockId = newParentBlockId;
        Order = newOrder;
        RaiseDomainEvent(new BlockMovedEvent(Id, newParentBlockId, newOrder));
    }

    public void ToggleCollapse() => IsCollapsed = !IsCollapsed;

    public void ChangeType(BlockType type) => Type = type;

    public void AddTag(Guid tagId)
    {
        if (!_tagIds.Contains(tagId)) _tagIds.Add(tagId);
    }

    public void RemoveTag(Guid tagId) => _tagIds.Remove(tagId);

    public void MarkDeleted()
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
    }
}
