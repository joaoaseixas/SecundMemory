using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Common;
using SecundMemory.Application.Features.Tags.Dtos;
using SecundMemory.Domain.Blocks;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Tags.Queries.GetBlocksByTag;

/// <summary>
/// The "instances of this class" view that makes tags-as-classes tangible:
/// every block carrying this tag, with the page it lives on and its current
/// property values, so a tag's detail page can render them like table rows.
/// </summary>
public class GetBlocksByTagHandler(IBlockRepository blockRepository, IPageRepository pageRepository, IApplicationDbContext db)
    : IQueryHandler<GetBlocksByTagQuery, IReadOnlyList<TaggedBlockDto>>
{
    public async Task<IReadOnlyList<TaggedBlockDto>> Handle(GetBlocksByTagQuery query, CancellationToken cancellationToken)
    {
        var blocks = await blockRepository.GetByTagIdAsync(query.WorkspaceId, query.TagId, cancellationToken);
        if (blocks.Count == 0) return [];

        var pages = await pageRepository.GetByIdsAsync(blocks.Select(b => b.PageId), cancellationToken);
        var pageTitleById = pages.ToDictionary(p => p.Id, p => p.Title.Value);

        var propertiesByBlock = await BlockPropertyLoader.LoadManyAsync(db, blocks.Select(b => b.Id).ToList(), cancellationToken);

        return blocks
            .Select(b => new TaggedBlockDto(
                b.Id, b.PageId, pageTitleById.GetValueOrDefault(b.PageId, string.Empty), b.Content.Value, propertiesByBlock[b.Id].ToList()))
            .ToList();
    }
}
