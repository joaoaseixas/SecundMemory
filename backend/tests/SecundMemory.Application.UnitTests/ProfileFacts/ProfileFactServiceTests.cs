using SecundMemory.Application.ProfileFacts;
using Xunit;

namespace SecundMemory.Application.UnitTests.ProfileFacts;

public class ProfileFactServiceTests
{
    [Fact]
    public async Task UpsertAsync_CreatesFact_WhenKeyDoesNotExist()
    {
        using var db = TestApplicationDbContext.Create();
        var sut = new ProfileFactService(db);

        var fact = await sut.UpsertAsync(new UpsertProfileFactRequest("preferences", "editor", "Claude Code"));

        Assert.Equal("Claude Code", fact.Value);
    }

    [Fact]
    public async Task UpsertAsync_UpdatesExistingValue_InsteadOfCreatingDuplicate()
    {
        using var db = TestApplicationDbContext.Create();
        var sut = new ProfileFactService(db);
        await sut.UpsertAsync(new UpsertProfileFactRequest("preferences", "editor", "VS Code"));

        var updated = await sut.UpsertAsync(new UpsertProfileFactRequest("preferences", "editor", "Claude Code"));

        var all = await sut.GetAllAsync("preferences");
        Assert.Single(all);
        Assert.Equal("Claude Code", updated.Value);
    }
}
