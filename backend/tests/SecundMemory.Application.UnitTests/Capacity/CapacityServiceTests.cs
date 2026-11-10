using SecundMemory.Application.Capacity;
using SecundMemory.Application.Notes;
using SecundMemory.Application.ProfileFacts;
using Xunit;

namespace SecundMemory.Application.UnitTests.Capacity;

public class CapacityServiceTests
{
    [Fact]
    public async Task GetSummaryAsync_CountsNotesTagsAndProfileFacts()
    {
        using var db = TestApplicationDbContext.Create();
        await new NoteService(db).CreateAsync(new CreateNoteRequest("Note A", "...", ["tag-1", "tag-2"]));
        await new ProfileFactService(db).UpsertAsync(new UpsertProfileFactRequest("bio", "name", "Joao"));

        var summary = await new CapacityService(db).GetSummaryAsync();

        Assert.Equal(1, summary.NoteCount);
        Assert.Equal(2, summary.TagCount);
        Assert.Equal(1, summary.ProfileFactCount);
    }
}
