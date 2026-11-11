using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Auth.Dtos;

namespace SecundMemory.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Simplified refresh: re-issues a token from an existing (possibly expired,
/// but signature-valid) one, rather than tracking separate refresh-token
/// records. Good enough for a single-user/small-workspace app; revisit with
/// real revocable refresh tokens if multi-device session management becomes
/// a real requirement.
/// </summary>
public record RefreshTokenCommand(string ExpiredAccessToken) : ICommand<AuthResultDto>;
