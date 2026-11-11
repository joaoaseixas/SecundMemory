using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Journals.Common;
using SecundMemory.Application.Features.Journals.Dtos;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Journals.Commands.OpenJournalByDate;

/// <summary>
/// Backs navigating the journal to any date (past catch-up or future
/// planning, both valid in Logseq) without the dead-end "no entry for this
/// day" the read-only GetJournalByDateQuery alone would force on the UI.
/// </summary>
public class OpenJournalByDateHandler(IApplicationDbContext db, IPageRepository pageRepository)
    : ICommandHandler<OpenJournalByDateCommand, JournalDto>
{
    public Task<JournalDto> Handle(OpenJournalByDateCommand command, CancellationToken cancellationToken) =>
        JournalOpener.GetOrCreateAsync(db, pageRepository, command.WorkspaceId, command.Date, cancellationToken);
}
