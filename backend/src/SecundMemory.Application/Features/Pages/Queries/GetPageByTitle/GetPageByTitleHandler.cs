using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Pages.Dtos;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Pages.Queries.GetPageByTitle;

/// <summary>
/// Returns null (not NotFoundException) — looking up a page by title is how
/// the editor decides whether linking to [[Page Title]] should auto-create
/// it, so "doesn't exist yet" is an expected, normal outcome here.
/// </summary>
public class GetPageByTitleHandler(IPageRepository pageRepository) : IQueryHandler<GetPageByTitleQuery, PageDto?>
{
    public async Task<PageDto?> Handle(GetPageByTitleQuery query, CancellationToken cancellationToken)
    {
        var page = await pageRepository.GetByTitleAsync(query.WorkspaceId, query.Title, cancellationToken);
        return page?.ToDto();
    }
}
