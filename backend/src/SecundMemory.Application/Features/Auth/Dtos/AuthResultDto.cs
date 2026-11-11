namespace SecundMemory.Application.Features.Auth.Dtos;

public record AuthResultDto(Guid UserId, Guid WorkspaceId, string AccessToken, DateTime ExpiresAtUtc);
