using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Journals.Dtos;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Journals.Queries.GetJournalByDate;

public class GetJournalByDateHandler(IApplicationDbContext db, IPageRepository pageRepository)
    : IQueryHandler<GetJournalByDateQuery, JournalDto?>
{
    public async Task<JournalDto?> Handle(GetJournalByDateQuery query, CancellationToken cancellationToken)
    {
        var journal = await db.Journals
            .FirstOrDefaultAsync(j => j.WorkspaceId == query.WorkspaceId && j.Date == query.Date, cancellationToken);

        if (journal is null) return null;

        var page = await pageRepository.GetByIdAsync(journal.PageId, cancellationToken);
        return new JournalDto(journal.Id, journal.WorkspaceId, journal.PageId, page?.Title.Value ?? string.Empty, journal.Date);
    }
}
