using SecundMemory.Domain.Exceptions;
using SecundMemory.Domain.Workspaces;

namespace SecundMemory.Domain.UnitTests.Workspaces;

public class WorkspaceTests
{
    [Fact]
    public void Create_AddsCreatorAsOwner()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create("My Brain", ownerId);

        var member = Assert.Single(workspace.Members);
        Assert.Equal(ownerId, member.UserId);
        Assert.Equal(WorkspaceRole.Owner, member.Role);
    }

    [Fact]
    public void RemoveMember_LastOwner_Throws()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create("My Brain", ownerId);

        Assert.Throws<BusinessRuleException>(() => workspace.RemoveMember(ownerId));
    }

    [Fact]
    public void AddMember_Duplicate_Throws()
    {
        var ownerId = Guid.NewGuid();
        var workspace = Workspace.Create("My Brain", ownerId);

        Assert.Throws<BusinessRuleException>(() => workspace.AddMember(ownerId, WorkspaceRole.Member));
    }

    [Fact]
    public void RemoveMember_SecondOwner_Succeeds()
    {
        var ownerId = Guid.NewGuid();
        var secondOwnerId = Guid.NewGuid();
        var workspace = Workspace.Create("My Brain", ownerId);
        workspace.AddMember(secondOwnerId, WorkspaceRole.Owner);

        workspace.RemoveMember(secondOwnerId);

        Assert.Single(workspace.Members);
    }
}
