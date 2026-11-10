using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SecundMemory.Domain.Entities;
using SecundMemory.Infrastructure.Persistence;
using Xunit;

namespace SecundMemory.Infrastructure.IntegrationTests.Persistence;

/// <summary>
/// Exercises the real ApplicationDbContext mapping against a relational engine
/// (SQLite in-memory) instead of the production Npgsql provider, so these tests
/// never need a reachable Postgres server — only the SQL semantics matter here.
/// </summary>
public class ApplicationDbContextTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly ApplicationDbContext _db;

    public ApplicationDbContextTests()
    {
        _connection.Open();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new ApplicationDbContext(options);
        _db.Database.EnsureCreated();
    }

    [Fact]
    public async Task SavingNoteWithTags_CreatesManyToManyRows()
    {
        var tag = new Tag { Name = "architecture" };
        var note = new Note { Title = "Clean Architecture", Content = "Entities -> Use Cases -> Interface Adapters -> Frameworks" };
        note.Tags.Add(tag);

        _db.Notes.Add(note);
        await _db.SaveChangesAsync();

        var reloaded = await _db.Notes.Include(n => n.Tags).SingleAsync(n => n.Id == note.Id);
        Assert.Single(reloaded.Tags);
        Assert.Equal("architecture", reloaded.Tags.First().Name);
    }

    [Fact]
    public async Task TagName_MustBeUnique()
    {
        _db.Tags.Add(new Tag { Name = "duplicate" });
        await _db.SaveChangesAsync();

        _db.Tags.Add(new Tag { Name = "duplicate" });

        await Assert.ThrowsAsync<DbUpdateException>(() => _db.SaveChangesAsync());
    }

    [Fact]
    public async Task ProfileFact_CategoryAndKey_MustBeUniqueTogether()
    {
        _db.ProfileFacts.Add(new ProfileFact { Category = "bio", Key = "name", Value = "Joao" });
        await _db.SaveChangesAsync();

        _db.ProfileFacts.Add(new ProfileFact { Category = "bio", Key = "name", Value = "Someone else" });

        await Assert.ThrowsAsync<DbUpdateException>(() => _db.SaveChangesAsync());
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
