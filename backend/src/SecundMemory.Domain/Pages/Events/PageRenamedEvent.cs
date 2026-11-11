using SecundMemory.Domain.Common;

namespace SecundMemory.Domain.Pages.Events;

public sealed class PageRenamedEvent(Guid pageId, string oldTitle, string newTitle) : IDomainEvent
{
    public Guid PageId { get; } = pageId;
    public string OldTitle { get; } = oldTitle;
    public string NewTitle { get; } = newTitle;
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
