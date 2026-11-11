using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Auth.Dtos;

namespace SecundMemory.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : ICommand<AuthResultDto>;
