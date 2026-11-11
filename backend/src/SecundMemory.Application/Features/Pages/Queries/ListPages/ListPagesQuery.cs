using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Models;
using SecundMemory.Application.Features.Pages.Dtos;

namespace SecundMemory.Application.Features.Pages.Queries.ListPages;

public record ListPagesQuery(Guid WorkspaceId, PaginationQuery Pagination) : IQuery<PagedResult<PageDto>>;
