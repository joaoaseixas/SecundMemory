using Microsoft.EntityFrameworkCore;

namespace SecundMemory.Infrastructure.Persistence.Seed;

public static class DbInitializer
{
    /// <summary>
    /// useSqlite=true (Api.IntegrationTests) skips migrations in favor of
    /// EnsureCreated: the committed migration's snapshot was generated
    /// against Npgsql, and comparing it against SQLite's own type mapping
    /// trips EF's pending-model-changes check even though nothing in the
    /// C# model actually changed. Tests care that the schema matches the
    /// current model, not that migration history replays cleanly under a
    /// provider production never uses.
    /// </summary>
    public static Task MigrateAsync(ApplicationDbContext db, bool useSqlite = false, CancellationToken cancellationToken = default) =>
        useSqlite ? db.Database.EnsureCreatedAsync(cancellationToken) : db.Database.MigrateAsync(cancellationToken);

    /// <summary>No seed data yet — hook kept separate from MigrateAsync so a future demo/dev dataset doesn't get tangled up with schema migration.</summary>
    public static Task SeedAsync(ApplicationDbContext db, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
