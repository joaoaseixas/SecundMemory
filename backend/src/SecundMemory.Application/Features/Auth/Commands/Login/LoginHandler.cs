using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Auth.Dtos;
using SecundMemory.Domain.Users;

namespace SecundMemory.Application.Features.Auth.Commands.Login;

public class LoginHandler(
    IApplicationDbContext db,
    IPasswordHasher passwordHasher,
    IJwtTokenService tokenService) : ICommandHandler<LoginCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var email = Email.Create(command.Email);
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var workspace = await db.Workspaces
            .FirstOrDefaultAsync(w => w.Members.Any(m => m.UserId == user.Id), cancellationToken);

        if (workspace is null)
            throw new UnauthorizedAccessException("This user does not belong to any workspace.");

        var roles = workspace.Members.Single(m => m.UserId == user.Id).Role.ToString();
        var token = tokenService.GenerateToken(user.Id, workspace.Id, [roles]);

        return new AuthResultDto(user.Id, workspace.Id, token.Value, token.ExpiresAtUtc);
    }
}
