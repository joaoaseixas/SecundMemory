using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Journals.Common;
using SecundMemory.Application.Features.Journals.Dtos;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Journals.Commands.OpenTodayJournal;

public class OpenTodayJournalHandler(
    IApplicationDbContext db,
    IPageRepository pageRepository,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<OpenTodayJournalCommand, JournalDto>
{
    public Task<JournalDto> Handle(OpenTodayJournalCommand command, CancellationToken cancellationToken) =>
        JournalOpener.GetOrCreateAsync(db, pageRepository, command.WorkspaceId, dateTimeProvider.Today, cancellationToken);
}
