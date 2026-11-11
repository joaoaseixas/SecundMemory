using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Application.Features.Workspaces.Dtos;
using SecundMemory.Domain.Workspaces;

namespace SecundMemory.Application.Features.Workspaces.Commands.AddMember;

public class AddMemberHandler(IApplicationDbContext db) : ICommandHandler<AddMemberCommand, WorkspaceDto>
{
    public async Task<WorkspaceDto> Handle(AddMemberCommand command, CancellationToken cancellationToken)
    {
        var workspace = await db.Workspaces.FirstOrDefaultAsync(w => w.Id == command.WorkspaceId, cancellationToken)
            ?? throw new NotFoundException(nameof(Workspace), command.WorkspaceId);

        workspace.AddMember(command.UserId, command.Role);

        return workspace.ToDto();
    }
}
