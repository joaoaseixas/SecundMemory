using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Auth.Dtos;

namespace SecundMemory.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string DisplayName, string Password) : ICommand<AuthResultDto>;
