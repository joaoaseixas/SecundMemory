using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Journals.Dtos;

namespace SecundMemory.Application.Features.Journals.Commands.OpenJournalByDate;

public record OpenJournalByDateCommand(Guid WorkspaceId, DateOnly Date) : ICommand<JournalDto>;
