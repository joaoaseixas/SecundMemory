using SecundMemory.Domain.Common;
using SecundMemory.Domain.Exceptions;

namespace SecundMemory.Domain.Workspaces;

public class Workspace : AggregateRoot
{
    private readonly List<WorkspaceMember> _members = [];

    public string Name { get; private set; } = string.Empty;
    public IReadOnlyCollection<WorkspaceMember> Members => _members.AsReadOnly();

    private Workspace() { }

    public static Workspace Create(string name, Guid ownerUserId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Workspace name cannot be empty.", nameof(name));

        var workspace = new Workspace { Name = name.Trim() };
        workspace._members.Add(WorkspaceMember.Create(workspace.Id, ownerUserId, WorkspaceRole.Owner));
        return workspace;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Workspace name cannot be empty.", nameof(name));

        Name = name.Trim();
    }

    public void AddMember(Guid userId, WorkspaceRole role)
    {
        if (_members.Any(m => m.UserId == userId))
            throw new BusinessRuleException("MemberAlreadyExists", "This user is already a member of the workspace.");

        _members.Add(WorkspaceMember.Create(Id, userId, role));
    }

    public void RemoveMember(Guid userId)
    {
        var member = _members.SingleOrDefault(m => m.UserId == userId)
            ?? throw new BusinessRuleException("MemberNotFound", "This user is not a member of the workspace.");

        if (member.Role == WorkspaceRole.Owner && _members.Count(m => m.Role == WorkspaceRole.Owner) == 1)
            throw new BusinessRuleException("LastOwner", "A workspace must always have at least one owner.");

        _members.Remove(member);
    }
}
