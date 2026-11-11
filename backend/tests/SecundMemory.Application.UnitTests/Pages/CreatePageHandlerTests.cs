using SecundMemory.Application.Features.Pages.Commands.CreatePage;
using SecundMemory.Application.UnitTests.Fakes;

namespace SecundMemory.Application.UnitTests.Pages;

public class CreatePageHandlerTests
{
    [Fact]
    public async Task Handle_CreatesPage_WithGivenTitle()
    {
        var repository = new FakePageRepository();
        var sut = new CreatePageHandler(repository);
        var workspaceId = Guid.NewGuid();

        var result = await sut.Handle(new CreatePageCommand(workspaceId, "Projeto X"), CancellationToken.None);

        Assert.Equal("Projeto X", result.Title);
        Assert.Equal(workspaceId, result.WorkspaceId);
    }

    [Fact]
    public async Task Handle_DuplicateTitleInSameWorkspace_Throws()
    {
        var repository = new FakePageRepository();
        var sut = new CreatePageHandler(repository);
        var workspaceId = Guid.NewGuid();
        await sut.Handle(new CreatePageCommand(workspaceId, "Duplicate"), CancellationToken.None);

        await Assert.ThrowsAsync<SecundMemory.Application.Common.Exceptions.ValidationException>(
            () => sut.Handle(new CreatePageCommand(workspaceId, "Duplicate"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_JournalFlagAndDate_ArePreserved()
    {
        var repository = new FakePageRepository();
        var sut = new CreatePageHandler(repository);
        var date = new DateOnly(2026, 11, 11);

        var result = await sut.Handle(new CreatePageCommand(Guid.NewGuid(), "2026-11-11", IsJournal: true, JournalDate: date), CancellationToken.None);

        Assert.True(result.IsJournal);
        Assert.Equal(date, result.JournalDate);
    }
}
