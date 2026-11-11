using SecundMemory.Domain.Common;

namespace SecundMemory.Domain.Journals;

/// <summary>
/// A fast "journal for date X" index. The actual content is a normal
/// Page+Blocks tree (created first); Journal just pins a Page to a calendar
/// date for the Fase 1 "open today's journal" / "navigate by date" use case,
/// so content storage has a single source of truth.
/// </summary>
public class Journal : AggregateRoot
{
    public Guid WorkspaceId { get; private init; }
    public Guid PageId { get; private init; }
    public DateOnly Date { get; private init; }

    private Journal() { }

    public static Journal Create(Guid workspaceId, Guid pageId, DateOnly date) => new()
    {
        WorkspaceId = workspaceId,
        PageId = pageId,
        Date = date,
    };
}
