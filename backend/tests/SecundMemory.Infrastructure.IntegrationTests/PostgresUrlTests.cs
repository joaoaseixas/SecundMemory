using SecundMemory.Infrastructure;

namespace SecundMemory.Infrastructure.IntegrationTests;

public class PostgresUrlTests
{
    [Fact]
    public void Url_is_converted_to_npgsql_format()
    {
        var result = DependencyInjection.NormalizePostgresUrl("postgresql://me:p%40ss@db.example.com/brain?sslmode=require");

        Assert.Contains("Host=db.example.com", result);
        Assert.Contains("Database=brain", result);
        Assert.Contains("Username=me", result);
        Assert.Contains("Password=p@ss", result);
        Assert.Contains("SSL Mode=Require", result);
    }

    [Fact]
    public void Key_value_strings_are_left_alone()
    {
        const string cs = "Host=localhost;Database=x;Username=u;Password=p";
        Assert.Equal(cs, DependencyInjection.NormalizePostgresUrl(cs));
    }
}
