using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Application.Features.Pages.Dtos;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Pages.Queries.GetPageById;

public class GetPageByIdHandler(IPageRepository pageRepository) : IQueryHandler<GetPageByIdQuery, PageDto>
{
    public async Task<PageDto> Handle(GetPageByIdQuery query, CancellationToken cancellationToken)
    {
        var page = await pageRepository.GetByIdAsync(query.PageId, cancellationToken)
            ?? throw new NotFoundException(nameof(Page), query.PageId);

        return page.ToDto();
    }
}
