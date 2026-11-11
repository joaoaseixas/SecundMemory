using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Features.Blocks.Commands.CreateBlock;
using SecundMemory.Application.Features.Tags.Commands.ApplyTagToBlock;
using SecundMemory.Application.UnitTests.Fakes;
using SecundMemory.Domain.Properties;
using SecundMemory.Domain.Tags;

namespace SecundMemory.Application.UnitTests.Tags;

public class ApplyTagToBlockHandlerTests
{
    [Fact]
    public async Task Handle_SeedsBlockPropertiesFromTagTemplates()
    {
        using var db = TestApplicationDbContext.Create();
        var blockRepository = new FakeBlockRepository();
        var block = await new CreateBlockHandler(blockRepository).Handle(new CreateBlockCommand(Guid.NewGuid(), null, "Dune"), CancellationToken.None);

        var tag = Tag.Create(Guid.NewGuid(), "book");
        tag.AddPropertyTemplate(new TagPropertyTemplate("status", PropertyType.Text, "to-read", allowMultiple: false));
        db.Tags.Add(tag);
        await db.SaveChangesAsync();

        var sut = new ApplyTagToBlockHandler(db, blockRepository);
        var result = await sut.Handle(new ApplyTagToBlockCommand(block.Id, tag.Id), CancellationToken.None);
        await db.SaveChangesAsync();

        Assert.Contains(tag.Id, result.TagIds);
        var property = await db.BlockProperties.SingleAsync(p => p.BlockId == block.Id);
        Assert.Equal("status", property.Key);
        Assert.Equal("to-read", property.Value);
    }

    [Fact]
    public async Task Handle_AppliedTwice_DoesNotDuplicateProperties()
    {
        using var db = TestApplicationDbContext.Create();
        var blockRepository = new FakeBlockRepository();
        var block = await new CreateBlockHandler(blockRepository).Handle(new CreateBlockCommand(Guid.NewGuid(), null, "Dune"), CancellationToken.None);
        var tag = Tag.Create(Guid.NewGuid(), "book");
        tag.AddPropertyTemplate(new TagPropertyTemplate("status", PropertyType.Text, "to-read", allowMultiple: false));
        db.Tags.Add(tag);
        await db.SaveChangesAsync();
        var sut = new ApplyTagToBlockHandler(db, blockRepository);
        await sut.Handle(new ApplyTagToBlockCommand(block.Id, tag.Id), CancellationToken.None);
        await db.SaveChangesAsync();

        await sut.Handle(new ApplyTagToBlockCommand(block.Id, tag.Id), CancellationToken.None);

        Assert.Equal(1, await db.BlockProperties.CountAsync(p => p.BlockId == block.Id));
    }
}
