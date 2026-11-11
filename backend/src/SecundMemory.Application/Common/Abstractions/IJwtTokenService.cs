namespace SecundMemory.Application.Common.Abstractions;

public record AccessToken(string Value, DateTime ExpiresAtUtc);

public record TokenPrincipal(Guid UserId, Guid WorkspaceId, IReadOnlyList<string> Roles);

public interface IJwtTokenService
{
    AccessToken GenerateToken(Guid userId, Guid workspaceId, IEnumerable<string> roles);

    /// <summary>
    /// Reads an existing token's claims without honoring its expiry — used
    /// only by the refresh flow to know who to re-issue a token for.
    /// </summary>
    TokenPrincipal? ReadPrincipalIgnoringExpiry(string token);
}
