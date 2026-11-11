namespace SecundMemory.Application.Common.Abstractions;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    Guid? UserId { get; }
    Guid? WorkspaceId { get; }
    IReadOnlyCollection<string> Roles { get; }
}
