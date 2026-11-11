using SecundMemory.Application.Common.Abstractions;

namespace SecundMemory.Application.UnitTests.Fakes;

/// <summary>Not real hashing — tests only need "hash then verify round-trips," never real security.</summary>
public class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hashed:{password}";

    public bool Verify(string password, string hash) => hash == $"hashed:{password}";
}
