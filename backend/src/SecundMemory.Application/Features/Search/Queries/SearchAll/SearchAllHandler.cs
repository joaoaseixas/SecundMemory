using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;

namespace SecundMemory.Application.Features.Search.Queries.SearchAll;

public class SearchAllHandler(ISearchService searchService) : IQueryHandler<SearchAllQuery, IReadOnlyList<SearchResult>>
{
    public Task<IReadOnlyList<SearchResult>> Handle(SearchAllQuery query, CancellationToken cancellationToken) =>
        searchService.SearchAsync(query.WorkspaceId, query.Query, query.Limit, cancellationToken);
}
