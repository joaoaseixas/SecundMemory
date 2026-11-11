using SecundMemory.Application.Features.Blocks.Commands.CreateBlock;
using SecundMemory.Application.Features.Blocks.Commands.DeleteBlock;
using SecundMemory.Application.UnitTests.Fakes;

namespace SecundMemory.Application.UnitTests.Blocks;

public class DeleteBlockHandlerTests
{
    [Fact]
    public async Task Handle_DeletesBlockAndItsChildren()
    {
        var repository = new FakeBlockRepository();
        var createHandler = new CreateBlockHandler(repository);
        var pageId = Guid.NewGuid();
        var parent = await createHandler.Handle(new CreateBlockCommand(pageId, null, "parent"), CancellationToken.None);
        var child = await createHandler.Handle(new CreateBlockCommand(pageId, parent.Id, "child"), CancellationToken.None);

        await new DeleteBlockHandler(repository).Handle(new DeleteBlockCommand(parent.Id), CancellationToken.None);

        Assert.True((await repository.GetByIdAsync(parent.Id))!.IsDeleted);
        Assert.True((await repository.GetByIdAsync(child.Id))!.IsDeleted);
    }
}
