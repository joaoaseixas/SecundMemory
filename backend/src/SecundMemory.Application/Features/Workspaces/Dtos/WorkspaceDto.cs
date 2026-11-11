using SecundMemory.Domain.Workspaces;

namespace SecundMemory.Application.Features.Workspaces.Dtos;

public record WorkspaceMemberDto(Guid UserId, WorkspaceRole Role);

public record WorkspaceDto(Guid Id, string Name, IReadOnlyList<WorkspaceMemberDto> Members);

public static class WorkspaceMappings
{
    public static WorkspaceDto ToDto(this Workspace workspace) => new(
        workspace.Id,
        workspace.Name,
        workspace.Members.Select(m => new WorkspaceMemberDto(m.UserId, m.Role)).ToList());
}
