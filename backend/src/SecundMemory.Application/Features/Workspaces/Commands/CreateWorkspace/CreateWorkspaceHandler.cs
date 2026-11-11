using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Workspaces.Dtos;
using SecundMemory.Domain.Workspaces;

namespace SecundMemory.Application.Features.Workspaces.Commands.CreateWorkspace;

public class CreateWorkspaceHandler(IApplicationDbContext db) : ICommandHandler<CreateWorkspaceCommand, WorkspaceDto>
{
    public Task<WorkspaceDto> Handle(CreateWorkspaceCommand command, CancellationToken cancellationToken)
    {
        var workspace = Workspace.Create(command.Name, command.OwnerUserId);
        db.Workspaces.Add(workspace);

        return Task.FromResult(workspace.ToDto());
    }
}
