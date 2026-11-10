using SecundMemory.Domain.Entities;
using Xunit;

namespace SecundMemory.Domain.UnitTests.Entities;

public class ProfileFactTests
{
    [Fact]
    public void NewProfileFact_HasUniqueId()
    {
        var first = new ProfileFact { Category = "bio", Key = "name", Value = "Joao" };
        var second = new ProfileFact { Category = "bio", Key = "role", Value = "Developer" };

        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(first.Id, second.Id);
    }
}
