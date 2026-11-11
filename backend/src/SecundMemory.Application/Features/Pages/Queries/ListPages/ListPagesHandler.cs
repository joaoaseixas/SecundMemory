using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Models;
using SecundMemory.Application.Features.Pages.Dtos;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Pages.Queries.ListPages;

public class ListPagesHandler(IPageRepository pageRepository) : IQueryHandler<ListPagesQuery, PagedResult<PageDto>>
{
    public async Task<PagedResult<PageDto>> Handle(ListPagesQuery query, CancellationToken cancellationToken)
    {
        var all = await pageRepository.ListAsync(query.WorkspaceId, cancellationToken);

        var page = all
            .Skip(query.Pagination.Skip)
            .Take(query.Pagination.PageSize)
            .Select(p => p.ToDto())
            .ToList();

        return new PagedResult<PageDto>(page, query.Pagination.Page, query.Pagination.PageSize, all.Count);
    }
}
