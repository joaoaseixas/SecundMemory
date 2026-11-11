using SecundMemory.Application.Common.Abstractions;

namespace SecundMemory.Application.UnitTests.Fakes;

public class FakeJwtTokenService : IJwtTokenService
{
    public AccessToken GenerateToken(Guid userId, Guid workspaceId, IEnumerable<string> roles) =>
        new($"{userId}|{workspaceId}|{string.Join(',', roles)}", DateTime.UtcNow.AddHours(1));

    public TokenPrincipal? ReadPrincipalIgnoringExpiry(string token)
    {
        var parts = token.Split('|');
        if (parts.Length != 3) return null;

        var roles = parts[2].Length == 0 ? [] : parts[2].Split(',').ToList();
        return new TokenPrincipal(Guid.Parse(parts[0]), Guid.Parse(parts[1]), roles);
    }
}
