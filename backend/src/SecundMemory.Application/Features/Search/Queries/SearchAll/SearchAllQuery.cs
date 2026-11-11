using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;

namespace SecundMemory.Application.Features.Search.Queries.SearchAll;

public record SearchAllQuery(Guid WorkspaceId, string Query, int Limit = 20) : IQuery<IReadOnlyList<SearchResult>>;
