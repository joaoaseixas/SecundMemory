using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Workspaces.Dtos;

namespace SecundMemory.Application.Features.Workspaces.Commands.CreateWorkspace;

public record CreateWorkspaceCommand(string Name, Guid OwnerUserId) : ICommand<WorkspaceDto>;
