using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Auth.Dtos;

namespace SecundMemory.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenHandler(IJwtTokenService tokenService) : ICommandHandler<RefreshTokenCommand, AuthResultDto>
{
    public Task<AuthResultDto> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var principal = tokenService.ReadPrincipalIgnoringExpiry(command.ExpiredAccessToken)
            ?? throw new UnauthorizedAccessException("The provided token is not valid.");

        var token = tokenService.GenerateToken(principal.UserId, principal.WorkspaceId, principal.Roles);
        return Task.FromResult(new AuthResultDto(principal.UserId, principal.WorkspaceId, token.Value, token.ExpiresAtUtc));
    }
}
