using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Pages.Dtos;
using SecundMemory.Domain.Blocks;
using SecundMemory.Domain.Links;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Pages.Queries.GetBacklinks;

public class GetBacklinksHandler(
    IApplicationDbContext db,
    IBlockRepository blockRepository,
    IPageRepository pageRepository) : IQueryHandler<GetBacklinksQuery, IReadOnlyList<BacklinkDto>>
{
    private const int SnippetMaxLength = 160;

    public async Task<IReadOnlyList<BacklinkDto>> Handle(GetBacklinksQuery query, CancellationToken cancellationToken)
    {
        var references = await db.BlockReferences
            .Where(r => r.TargetPageId == query.PageId && r.Type == BlockReferenceType.PageLink)
            .ToListAsync(cancellationToken);

        if (references.Count == 0) return [];

        var sourceBlocks = await blockRepository.GetByIdsAsync(references.Select(r => r.SourceBlockId), cancellationToken);
        var sourcePages = await pageRepository.GetByIdsAsync(sourceBlocks.Select(b => b.PageId), cancellationToken);
        var pagesById = sourcePages.ToDictionary(p => p.Id);

        return sourceBlocks
            .Where(block => pagesById.ContainsKey(block.PageId))
            .Select(block =>
            {
                var page = pagesById[block.PageId];
                var content = block.Content.Value;
                var snippet = content.Length > SnippetMaxLength ? content[..SnippetMaxLength] + "…" : content;
                return new BacklinkDto(block.Id, page.Id, page.Title.Value, snippet);
            })
            .ToList();
    }
}
