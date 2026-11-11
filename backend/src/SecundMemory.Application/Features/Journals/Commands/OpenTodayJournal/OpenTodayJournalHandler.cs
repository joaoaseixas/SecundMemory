using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Journals.Dtos;
using SecundMemory.Domain.Journals;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Journals.Commands.OpenTodayJournal;

public class OpenTodayJournalHandler(
    IApplicationDbContext db,
    IPageRepository pageRepository,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<OpenTodayJournalCommand, JournalDto>
{
    public async Task<JournalDto> Handle(OpenTodayJournalCommand command, CancellationToken cancellationToken)
    {
        var today = dateTimeProvider.Today;

        var journal = await db.Journals
            .FirstOrDefaultAsync(j => j.WorkspaceId == command.WorkspaceId && j.Date == today, cancellationToken);

        if (journal is not null)
        {
            var existingPage = await pageRepository.GetByIdAsync(journal.PageId, cancellationToken);
            return new JournalDto(journal.Id, journal.WorkspaceId, journal.PageId, existingPage!.Title.Value, journal.Date);
        }

        var title = today.ToString("yyyy-MM-dd");
        var page = Page.Create(command.WorkspaceId, PageTitle.Create(title), isJournal: true, journalDate: today);
        pageRepository.Add(page);

        journal = Journal.Create(command.WorkspaceId, page.Id, today);
        db.Journals.Add(journal);

        return new JournalDto(journal.Id, journal.WorkspaceId, journal.PageId, title, today);
    }
}
