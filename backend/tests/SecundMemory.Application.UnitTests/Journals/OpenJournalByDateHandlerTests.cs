using SecundMemory.Application.Features.Journals.Commands.OpenJournalByDate;
using SecundMemory.Application.UnitTests.Fakes;

namespace SecundMemory.Application.UnitTests.Journals;

public class OpenJournalByDateHandlerTests
{
    [Fact]
    public async Task Handle_NoExistingJournal_CreatesPageAndJournal()
    {
        using var db = TestApplicationDbContext.Create();
        var pageRepository = new FakePageRepository();
        var sut = new OpenJournalByDateHandler(db, pageRepository);
        var workspaceId = Guid.NewGuid();
        var date = new DateOnly(2026, 11, 12);

        var result = await sut.Handle(new OpenJournalByDateCommand(workspaceId, date), CancellationToken.None);
        await db.SaveChangesAsync();

        Assert.Equal("2026-11-12", result.PageTitle);
        Assert.Equal(date, result.Date);
        var createdPage = await pageRepository.GetByIdAsync(result.PageId);
        Assert.NotNull(createdPage);
        Assert.True(createdPage!.IsJournal);
        Assert.Equal(date, createdPage.JournalDate);
    }

    [Fact]
    public async Task Handle_JournalAlreadyExistsForThatDate_ReturnsSameOneInstead()
    {
        using var db = TestApplicationDbContext.Create();
        var pageRepository = new FakePageRepository();
        var sut = new OpenJournalByDateHandler(db, pageRepository);
        var workspaceId = Guid.NewGuid();
        var date = new DateOnly(2026, 11, 12);

        var first = await sut.Handle(new OpenJournalByDateCommand(workspaceId, date), CancellationToken.None);
        await db.SaveChangesAsync();

        var second = await sut.Handle(new OpenJournalByDateCommand(workspaceId, date), CancellationToken.None);
        await db.SaveChangesAsync();

        Assert.Equal(first.Id, second.Id);
        Assert.Equal(first.PageId, second.PageId);
        Assert.Single(await pageRepository.ListAsync(workspaceId));
    }

    [Fact]
    public async Task Handle_FutureDate_IsAllowed()
    {
        using var db = TestApplicationDbContext.Create();
        var pageRepository = new FakePageRepository();
        var sut = new OpenJournalByDateHandler(db, pageRepository);
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1);

        var result = await sut.Handle(new OpenJournalByDateCommand(Guid.NewGuid(), futureDate), CancellationToken.None);

        Assert.Equal(futureDate, result.Date);
    }
}
