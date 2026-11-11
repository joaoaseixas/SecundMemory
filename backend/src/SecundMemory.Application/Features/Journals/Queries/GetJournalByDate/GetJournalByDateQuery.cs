using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Journals.Dtos;

namespace SecundMemory.Application.Features.Journals.Queries.GetJournalByDate;

public record GetJournalByDateQuery(Guid WorkspaceId, DateOnly Date) : IQuery<JournalDto?>;
