using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Features.Journals.Dtos;
using SecundMemory.Domain.Journals;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Journals.Common;

/// <summary>
/// Shared get-or-create logic behind both "open today's journal" and "open
/// the journal for an arbitrary date" — a journal is just a Page with
/// IsJournal=true plus a Journal row indexing it by date, and both commands
/// need the same idempotent lookup-or-create.
/// </summary>
internal static class JournalOpener
{
    public static async Task<JournalDto> GetOrCreateAsync(
        IApplicationDbContext db, IPageRepository pageRepository, Guid workspaceId, DateOnly date, CancellationToken cancellationToken)
    {
        var journal = await db.Journals
            .FirstOrDefaultAsync(j => j.WorkspaceId == workspaceId && j.Date == date, cancellationToken);

        if (journal is not null)
        {
            var existingPage = await pageRepository.GetByIdAsync(journal.PageId, cancellationToken);
            return new JournalDto(journal.Id, journal.WorkspaceId, journal.PageId, existingPage!.Title.Value, journal.Date);
        }

        var title = date.ToString("yyyy-MM-dd");
        var page = Page.Create(workspaceId, PageTitle.Create(title), isJournal: true, journalDate: date);
        pageRepository.Add(page);

        journal = Journal.Create(workspaceId, page.Id, date);
        db.Journals.Add(journal);

        return new JournalDto(journal.Id, journal.WorkspaceId, journal.PageId, title, date);
    }
}
