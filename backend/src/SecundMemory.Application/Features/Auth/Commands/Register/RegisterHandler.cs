using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Application.Features.Auth.Dtos;
using SecundMemory.Domain.Users;
using SecundMemory.Domain.Workspaces;

namespace SecundMemory.Application.Features.Auth.Commands.Register;

public class RegisterHandler(
    IApplicationDbContext db,
    IPasswordHasher passwordHasher,
    IJwtTokenService tokenService) : ICommandHandler<RegisterCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var email = Email.Create(command.Email);

        var alreadyExists = await db.Users.AnyAsync(u => u.Email == email, cancellationToken);
        if (alreadyExists)
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(nameof(command.Email), "An account with this email already exists.")]);

        var user = User.Register(email, command.DisplayName, passwordHasher.Hash(command.Password));
        var workspace = Workspace.Create($"{command.DisplayName}'s Second Brain", user.Id);

        db.Users.Add(user);
        db.Workspaces.Add(workspace);

        var token = tokenService.GenerateToken(user.Id, workspace.Id, ["Owner"]);
        return new AuthResultDto(user.Id, workspace.Id, token.Value, token.ExpiresAtUtc);
    }
}
