using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Features.Blocks.EventHandlers;
using SecundMemory.Application.UnitTests.Fakes;
using SecundMemory.Domain.Blocks;
using SecundMemory.Domain.Blocks.Events;
using SecundMemory.Domain.Links;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.UnitTests.Blocks;

public class BlockContentIndexingEventHandlerTests
{
    [Fact]
    public async Task Handle_LinkToNewPage_AutoCreatesThatPage()
    {
        using var db = TestApplicationDbContext.Create();
        var pageRepository = new FakePageRepository();
        var workspaceId = Guid.NewGuid();
        var sourcePage = Page.Create(workspaceId, PageTitle.Create("Source"));
        pageRepository.Add(sourcePage);
        var sut = new BlockContentIndexingEventHandler(db, pageRepository, new FakeBlockRepository());

        var blockId = Guid.NewGuid();
        await sut.Handle(new BlockContentUpdatedEvent(blockId, sourcePage.Id, "See [[Brand New Page]] for details."), CancellationToken.None);

        var createdPage = await pageRepository.GetByTitleAsync(workspaceId, "Brand New Page");
        Assert.NotNull(createdPage);
    }

    [Fact]
    public async Task Handle_LinkToExistingPage_DoesNotDuplicateIt()
    {
        using var db = TestApplicationDbContext.Create();
        var pageRepository = new FakePageRepository();
        var workspaceId = Guid.NewGuid();
        var sourcePage = Page.Create(workspaceId, PageTitle.Create("Source"));
        var targetPage = Page.Create(workspaceId, PageTitle.Create("Existing"));
        pageRepository.Add(sourcePage);
        pageRepository.Add(targetPage);
        var sut = new BlockContentIndexingEventHandler(db, pageRepository, new FakeBlockRepository());

        await sut.Handle(new BlockContentUpdatedEvent(Guid.NewGuid(), sourcePage.Id, "See [[Existing]]."), CancellationToken.None);

        var allNamedExisting = (await pageRepository.ListAsync(workspaceId)).Count(p => p.Title.Value == "Existing");
        Assert.Equal(1, allNamedExisting);
    }

    [Fact]
    public async Task Handle_CreatesBlockReference_ForBacklinksToResolve()
    {
        using var db = TestApplicationDbContext.Create();
        var pageRepository = new FakePageRepository();
        var workspaceId = Guid.NewGuid();
        var sourcePage = Page.Create(workspaceId, PageTitle.Create("Source"));
        pageRepository.Add(sourcePage);
        var sut = new BlockContentIndexingEventHandler(db, pageRepository, new FakeBlockRepository());
        var blockId = Guid.NewGuid();

        await sut.Handle(new BlockContentUpdatedEvent(blockId, sourcePage.Id, "See [[Target]]."), CancellationToken.None);
        await db.SaveChangesAsync();

        var targetPage = await pageRepository.GetByTitleAsync(workspaceId, "Target");
        var reference = await db.BlockReferences.SingleAsync();
        Assert.Equal(blockId, reference.SourceBlockId);
        Assert.Equal(targetPage!.Id, reference.TargetPageId);
        Assert.Equal(BlockReferenceType.PageLink, reference.Type);
    }

    [Fact]
    public async Task Handle_HashTags_CreateTagsAndSyncBlock()
    {
        using var db = TestApplicationDbContext.Create();
        var pageRepository = new FakePageRepository();
        var blockRepository = new FakeBlockRepository();
        var workspaceId = Guid.NewGuid();
        var page = Page.Create(workspaceId, PageTitle.Create("Source"));
        pageRepository.Add(page);
        var block = Block.Create(page.Id, null, BlockContent.Create("x"), 1);
        blockRepository.Add(block);
        var sut = new BlockContentIndexingEventHandler(db, pageRepository, blockRepository);

        await sut.Handle(new BlockContentUpdatedEvent(block.Id, page.Id, "idea #Book and #book #read-later"), CancellationToken.None);
        await db.SaveChangesAsync();

        Assert.Equal(2, await db.Tags.CountAsync());
        Assert.Equal(2, block.TagIds.Count);

        await sut.Handle(new BlockContentUpdatedEvent(block.Id, page.Id, "just #book now"), CancellationToken.None);
        Assert.Single(block.TagIds);
    }
}
