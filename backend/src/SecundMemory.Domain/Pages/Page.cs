using SecundMemory.Domain.Common;
using SecundMemory.Domain.Pages.Events;

namespace SecundMemory.Domain.Pages;

public class Page : AggregateRoot, ISoftDeletable
{
    // Stored as plain strings, not List<PageAlias>: EF Core treats any
    // custom type behind a collection as a navigable entity by convention,
    // regardless of a HasConversion configured for the collection property,
    // which broke model building for PageAlias (no usable constructor for
    // EF to materialize it as an owned/related type). Aliases are exposed
    // as PageAlias to callers; only the backing storage is a plain string.
    private readonly List<string> _aliasValues = [];

    public Guid WorkspaceId { get; private init; }
    public PageTitle Title { get; private set; } = null!;
    public IReadOnlyCollection<PageAlias> Aliases => _aliasValues.Select(PageAlias.Create).ToList();
    public bool IsJournal { get; private init; }
    public DateOnly? JournalDate { get; private init; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    /// <summary>
    /// Everything before the last '/' in the title, e.g. "projeto" for
    /// "projeto/subprojeto" — null for a title with no namespace segment.
    /// </summary>
    public string? Namespace => Title.Namespace;

    private Page() { }

    public static Page Create(Guid workspaceId, PageTitle title, bool isJournal = false, DateOnly? journalDate = null)
    {
        var page = new Page
        {
            WorkspaceId = workspaceId,
            Title = title,
            IsJournal = isJournal,
            JournalDate = journalDate,
        };

        page.RaiseDomainEvent(new PageCreatedEvent(page.Id, workspaceId, title.Value));
        return page;
    }

    public void Rename(PageTitle newTitle)
    {
        if (newTitle.Value == Title.Value) return;

        var oldTitle = Title.Value;
        Title = newTitle;
        RaiseDomainEvent(new PageRenamedEvent(Id, oldTitle, newTitle.Value));
    }

    public void AddAlias(PageAlias alias)
    {
        if (_aliasValues.Any(v => v.Equals(alias.Value, StringComparison.OrdinalIgnoreCase))) return;
        _aliasValues.Add(alias.Value);
    }

    public void RemoveAlias(PageAlias alias) =>
        _aliasValues.RemoveAll(v => v.Equals(alias.Value, StringComparison.OrdinalIgnoreCase));

    public void MarkDeleted()
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
    }
}
