using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Pages.Dtos;

public record PageDto(
    Guid Id,
    Guid WorkspaceId,
    string Title,
    string? Namespace,
    IReadOnlyList<string> Aliases,
    bool IsJournal,
    DateOnly? JournalDate,
    DateTime CreatedAtUtc,
    DateTime? LastModifiedAtUtc);

public static class PageMappings
{
    public static PageDto ToDto(this Page page) => new(
        page.Id,
        page.WorkspaceId,
        page.Title.Value,
        page.Namespace,
        page.Aliases.Select(a => a.Value).ToList(),
        page.IsJournal,
        page.JournalDate,
        page.CreatedAtUtc,
        page.LastModifiedAtUtc);
}
