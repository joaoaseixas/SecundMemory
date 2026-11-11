namespace SecundMemory.Application.Common.Abstractions;

public enum SearchResultKind
{
    Page,
    Block,
}

public record SearchResult(Guid Id, SearchResultKind Kind, Guid PageId, string Title, string Snippet, float Rank);

public interface ISearchService
{
    Task<IReadOnlyList<SearchResult>> SearchAsync(Guid workspaceId, string query, int limit = 20, CancellationToken cancellationToken = default);
}
