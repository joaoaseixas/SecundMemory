using SecundMemory.Domain.Common;

namespace SecundMemory.Domain.Pages.Events;

public sealed class PageCreatedEvent(Guid pageId, Guid workspaceId, string title) : IDomainEvent
{
    public Guid PageId { get; } = pageId;
    public Guid WorkspaceId { get; } = workspaceId;
    public string Title { get; } = title;
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
