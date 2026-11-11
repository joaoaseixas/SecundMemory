using SecundMemory.Application.Features.Blocks.Commands.ChangeBlockType;
using SecundMemory.Application.Features.Blocks.Commands.CreateBlock;
using SecundMemory.Application.UnitTests.Fakes;
using SecundMemory.Domain.Blocks;

namespace SecundMemory.Application.UnitTests.Blocks;

public class ChangeBlockTypeHandlerTests
{
    [Fact]
    public async Task Handle_ChangesType()
    {
        var repository = new FakeBlockRepository();
        var block = await new CreateBlockHandler(repository).Handle(new CreateBlockCommand(Guid.NewGuid(), null, "text"), CancellationToken.None);

        var result = await new ChangeBlockTypeHandler(repository).Handle(new ChangeBlockTypeCommand(block.Id, BlockType.Heading1), CancellationToken.None);

        Assert.Equal(BlockType.Heading1, result.Type);
    }
}
