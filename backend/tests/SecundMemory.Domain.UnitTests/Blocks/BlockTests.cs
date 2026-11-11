using SecundMemory.Domain.Blocks;
using SecundMemory.Domain.Blocks.Events;

namespace SecundMemory.Domain.UnitTests.Blocks;

public class BlockTests
{
    [Fact]
    public void Create_RaisesBlockCreatedEvent()
    {
        var block = Block.Create(Guid.NewGuid(), parentBlockId: null, BlockContent.Create("hello"), order: 1.0);

        Assert.IsType<BlockCreatedEvent>(Assert.Single(block.DomainEvents));
    }

    [Fact]
    public void UpdateContent_RaisesBlockContentUpdatedEvent_WithNewContent()
    {
        var block = Block.Create(Guid.NewGuid(), null, BlockContent.Create("old"), 1.0);
        block.ClearDomainEvents();

        block.UpdateContent(BlockContent.Create("new"));

        var @event = Assert.IsType<BlockContentUpdatedEvent>(Assert.Single(block.DomainEvents));
        Assert.Equal("new", @event.NewContent);
    }

    [Fact]
    public void UpdateContent_ToSameValue_RaisesNoEvent()
    {
        var block = Block.Create(Guid.NewGuid(), null, BlockContent.Create("same"), 1.0);
        block.ClearDomainEvents();

        block.UpdateContent(BlockContent.Create("same"));

        Assert.Empty(block.DomainEvents);
    }

    [Fact]
    public void Move_UpdatesParentAndOrder_AndRaisesEvent()
    {
        var block = Block.Create(Guid.NewGuid(), null, BlockContent.Create("x"), 1.0);
        var newParent = Guid.NewGuid();
        block.ClearDomainEvents();

        block.Move(newParent, 2.5);

        Assert.Equal(newParent, block.ParentBlockId);
        Assert.Equal(2.5, block.Order);
        Assert.IsType<BlockMovedEvent>(Assert.Single(block.DomainEvents));
    }

    [Fact]
    public void ToggleCollapse_FlipsState()
    {
        var block = Block.Create(Guid.NewGuid(), null, BlockContent.Create("x"), 1.0);

        block.ToggleCollapse();
        Assert.True(block.IsCollapsed);

        block.ToggleCollapse();
        Assert.False(block.IsCollapsed);
    }

    [Fact]
    public void AddTag_IsIdempotent()
    {
        var block = Block.Create(Guid.NewGuid(), null, BlockContent.Create("x"), 1.0);
        var tagId = Guid.NewGuid();

        block.AddTag(tagId);
        block.AddTag(tagId);

        Assert.Single(block.TagIds);
    }
}
