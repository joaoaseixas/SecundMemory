namespace SecundMemory.Application.Features.Pages.Dtos;

public record BacklinkDto(Guid SourceBlockId, Guid SourcePageId, string SourcePageTitle, string Snippet);
