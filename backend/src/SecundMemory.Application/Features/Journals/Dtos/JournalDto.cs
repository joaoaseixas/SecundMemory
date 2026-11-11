namespace SecundMemory.Application.Features.Journals.Dtos;

public record JournalDto(Guid Id, Guid WorkspaceId, Guid PageId, string PageTitle, DateOnly Date);
