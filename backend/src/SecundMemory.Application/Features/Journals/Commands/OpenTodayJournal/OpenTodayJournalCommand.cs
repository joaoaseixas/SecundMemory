using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Journals.Dtos;

namespace SecundMemory.Application.Features.Journals.Commands.OpenTodayJournal;

public record OpenTodayJournalCommand(Guid WorkspaceId) : ICommand<JournalDto>;
