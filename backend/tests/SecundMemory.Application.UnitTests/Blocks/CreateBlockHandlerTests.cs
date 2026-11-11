using SecundMemory.Application.Features.Blocks.Commands.CreateBlock;
using SecundMemory.Application.UnitTests.Fakes;

namespace SecundMemory.Application.UnitTests.Blocks;

public class CreateBlockHandlerTests
{
    [Fact]
    public async Task Handle_FirstRootBlock_GetsOrderOne()
    {
        var repository = new FakeBlockRepository();
        var sut = new CreateBlockHandler(repository);
        var pageId = Guid.NewGuid();

        var result = await sut.Handle(new CreateBlockCommand(pageId, null, "first"), CancellationToken.None);

        Assert.Equal(1.0, result.Order);
        Assert.Null(result.ParentBlockId);
    }

    [Fact]
    public async Task Handle_SecondRootBlock_OrdersAfterTheFirst()
    {
        var repository = new FakeBlockRepository();
        var sut = new CreateBlockHandler(repository);
        var pageId = Guid.NewGuid();
        await sut.Handle(new CreateBlockCommand(pageId, null, "first"), CancellationToken.None);

        var second = await sut.Handle(new CreateBlockCommand(pageId, null, "second"), CancellationToken.None);

        Assert.True(second.Order > 1.0);
    }

    [Fact]
    public async Task Handle_ChildBlock_OrdersAmongSiblingsOnly()
    {
        var repository = new FakeBlockRepository();
        var sut = new CreateBlockHandler(repository);
        var pageId = Guid.NewGuid();
        var parent = await sut.Handle(new CreateBlockCommand(pageId, null, "parent"), CancellationToken.None);

        var child = await sut.Handle(new CreateBlockCommand(pageId, parent.Id, "child"), CancellationToken.None);

        Assert.Equal(parent.Id, child.ParentBlockId);
        Assert.Equal(1.0, child.Order);
    }
}
