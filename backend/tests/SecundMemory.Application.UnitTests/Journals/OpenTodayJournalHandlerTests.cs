using SecundMemory.Application.Features.Journals.Commands.OpenTodayJournal;
using SecundMemory.Application.UnitTests.Fakes;

namespace SecundMemory.Application.UnitTests.Journals;

public class OpenTodayJournalHandlerTests
{
    [Fact]
    public async Task Handle_UsesDateTimeProviderToday_NotSystemClock()
    {
        using var db = TestApplicationDbContext.Create();
        var pageRepository = new FakePageRepository();
        var dateTimeProvider = new FakeDateTimeProvider(new DateTime(2026, 1, 1));
        var sut = new OpenTodayJournalHandler(db, pageRepository, dateTimeProvider);

        var result = await sut.Handle(new OpenTodayJournalCommand(Guid.NewGuid()), CancellationToken.None);
        await db.SaveChangesAsync();

        Assert.Equal(new DateOnly(2026, 1, 1), result.Date);
        Assert.Equal("2026-01-01", result.PageTitle);
    }

    [Fact]
    public async Task Handle_CalledTwiceSameDay_ReturnsSameJournal()
    {
        using var db = TestApplicationDbContext.Create();
        var pageRepository = new FakePageRepository();
        var dateTimeProvider = new FakeDateTimeProvider(new DateTime(2026, 1, 1));
        var sut = new OpenTodayJournalHandler(db, pageRepository, dateTimeProvider);
        var workspaceId = Guid.NewGuid();

        var first = await sut.Handle(new OpenTodayJournalCommand(workspaceId), CancellationToken.None);
        await db.SaveChangesAsync();
        var second = await sut.Handle(new OpenTodayJournalCommand(workspaceId), CancellationToken.None);

        Assert.Equal(first.PageId, second.PageId);
    }
}
