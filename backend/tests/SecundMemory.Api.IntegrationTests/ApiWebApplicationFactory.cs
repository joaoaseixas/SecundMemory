using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace SecundMemory.Api.IntegrationTests;

/// <summary>
/// Same reasoning as Infrastructure.IntegrationTests: SQLite instead of the
/// production Npgsql connection, so these tests need no reachable Postgres
/// server. The provider is chosen via Testing:UseSqlite (see
/// Infrastructure.DependencyInjection.AddPersistence) rather than swapped
/// after the fact, because removing just DbContextOptions&lt;T&gt; post-hoc
/// leaves Npgsql's own provider-service registrations behind, and EF Core
/// refuses to run with two providers registered at once.
///
/// A shared-cache SQLite URI keeps the same in-memory database alive across
/// every connection the app opens during a test, as long as one connection
/// to it (this one) stays open for the factory's lifetime.
/// </summary>
public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"apitests-{Guid.NewGuid():N}";
    private SqliteConnection? _keepAliveConnection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _keepAliveConnection = new SqliteConnection(ConnectionString);
        _keepAliveConnection.Open();

        builder.UseSetting("Testing:UseSqlite", "true");
        builder.UseSetting("ConnectionStrings:SecondBrain", ConnectionString);
        builder.UseSetting("Jwt:Secret", "integration-test-secret-only-never-used-in-prod-32ch");
    }

    private string ConnectionString => $"Data Source=file:{_dbName}?mode=memory&cache=shared";

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _keepAliveConnection?.Dispose();
    }
}
