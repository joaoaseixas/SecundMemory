using SecundMemory.Application.Notes;
using Xunit;

namespace SecundMemory.Application.UnitTests.Notes;

public class NoteServiceTests
{
    [Fact]
    public async Task CreateAsync_PersistsNoteWithResolvedTags()
    {
        using var db = TestApplicationDbContext.Create();
        var sut = new NoteService(db);

        var created = await sut.CreateAsync(new CreateNoteRequest("Second brain", "Why Clean Architecture", ["architecture", "notes"]));

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal(2, created.Tags.Count);
        Assert.Contains("architecture", created.Tags);
    }

    [Fact]
    public async Task CreateAsync_ReusesExistingTagInsteadOfDuplicating()
    {
        using var db = TestApplicationDbContext.Create();
        var sut = new NoteService(db);

        await sut.CreateAsync(new CreateNoteRequest("First", "...", ["shared-tag"]));
        await sut.CreateAsync(new CreateNoteRequest("Second", "...", ["shared-tag"]));

        Assert.Equal(1, db.Tags.Count(t => t.Name == "shared-tag"));
    }

    [Fact]
    public async Task SearchAsync_FiltersByTitleOrContent()
    {
        using var db = TestApplicationDbContext.Create();
        var sut = new NoteService(db);
        await sut.CreateAsync(new CreateNoteRequest("Obsidian vault", "personal knowledge base", []));
        await sut.CreateAsync(new CreateNoteRequest("Grocery list", "milk, eggs", []));

        var results = await sut.SearchAsync("vault");

        Assert.Single(results);
        Assert.Equal("Obsidian vault", results[0].Title);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNoteDoesNotExist()
    {
        using var db = TestApplicationDbContext.Create();
        var sut = new NoteService(db);

        var deleted = await sut.DeleteAsync(Guid.NewGuid());

        Assert.False(deleted);
    }
}
