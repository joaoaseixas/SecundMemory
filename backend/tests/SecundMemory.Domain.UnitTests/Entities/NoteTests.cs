using SecundMemory.Domain.Entities;
using Xunit;

namespace SecundMemory.Domain.UnitTests.Entities;

public class NoteTests
{
    [Fact]
    public void NewNote_HasUniqueId()
    {
        var first = new Note { Title = "First" };
        var second = new Note { Title = "Second" };

        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void NewNote_StartsWithNoTagsAndEmptyContent()
    {
        var note = new Note { Title = "Untitled" };

        Assert.Empty(note.Tags);
        Assert.Equal(string.Empty, note.Content);
    }

    [Fact]
    public void NewNote_CreatedAndUpdatedTimestampsStartEqual()
    {
        var note = new Note { Title = "Untitled" };

        Assert.Equal(note.CreatedUtc, note.UpdatedUtc, TimeSpan.FromSeconds(1));
    }
}
