using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Interfaces;
using SecundMemory.Domain.Entities;

namespace SecundMemory.Application.UnitTests;

/// <summary>
/// In-memory stand-in for IApplicationDbContext used only by Application-layer
/// tests, so those tests never depend on the real Infrastructure/Npgsql
/// implementation — proving the use cases are testable in isolation.
/// </summary>
public class TestApplicationDbContext : DbContext, IApplicationDbContext
{
    public TestApplicationDbContext(DbContextOptions<TestApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ProfileFact> ProfileFacts => Set<ProfileFact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>().HasMany(n => n.Tags).WithMany(t => t.Notes);
    }

    public static TestApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }
}
