using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Features.Blocks.EventHandlers;
using SecundMemory.Application.UnitTests.Fakes;
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
        var sut = new BlockContentIndexingEventHandler(db, pageRepository);

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
        var sut = new BlockContentIndexingEventHandler(db, pageRepository);

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
        var sut = new BlockContentIndexingEventHandler(db, pageRepository);
        var blockId = Guid.NewGuid();

        await sut.Handle(new BlockContentUpdatedEvent(blockId, sourcePage.Id, "See [[Target]]."), CancellationToken.None);
        await db.SaveChangesAsync();

        var targetPage = await pageRepository.GetByTitleAsync(workspaceId, "Target");
        var reference = await db.BlockReferences.SingleAsync();
        Assert.Equal(blockId, reference.SourceBlockId);
        Assert.Equal(targetPage!.Id, reference.TargetPageId);
        Assert.Equal(BlockReferenceType.PageLink, reference.Type);
    }
}
