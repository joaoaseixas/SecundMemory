using SecundMemory.Domain.Common;

namespace SecundMemory.Domain.Workspaces;

public enum WorkspaceRole
{
    Owner,
    Member,
}

/// <summary>
/// Child entity owned by Workspace — reached and mutated only through it,
/// never persisted or loaded on its own.
/// </summary>
public class WorkspaceMember : Entity
{
    public Guid WorkspaceId { get; private init; }
    public Guid UserId { get; private init; }
    public WorkspaceRole Role { get; private set; }
    public DateTime JoinedAtUtc { get; private init; }

    private WorkspaceMember() { }

    internal static WorkspaceMember Create(Guid workspaceId, Guid userId, WorkspaceRole role) => new()
    {
        WorkspaceId = workspaceId,
        UserId = userId,
        Role = role,
        JoinedAtUtc = DateTime.UtcNow,
    };

    internal void ChangeRole(WorkspaceRole role) => Role = role;
}
