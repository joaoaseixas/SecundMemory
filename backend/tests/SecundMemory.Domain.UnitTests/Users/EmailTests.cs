using SecundMemory.Domain.Exceptions;
using SecundMemory.Domain.Users;

namespace SecundMemory.Domain.UnitTests.Users;

public class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("A.B@Example.CO")]
    public void Create_WithValidAddress_NormalizesToLowercase(string input)
    {
        var email = Email.Create(input);

        Assert.Equal(input.ToLowerInvariant().Trim(), email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("missing@domain")]
    public void Create_WithInvalidAddress_Throws(string input)
    {
        Assert.Throws<BusinessRuleException>(() => Email.Create(input));
    }

    [Fact]
    public void Equality_IsCaseNormalizedValueBased()
    {
        var a = Email.Create("User@Example.com");
        var b = Email.Create("user@example.com");

        Assert.Equal(a, b);
    }
}
