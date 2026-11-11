using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Graph.Dtos;
using SecundMemory.Domain.Blocks;
using SecundMemory.Domain.Links;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Graph.Queries.GetGraphData;

/// <summary>
/// Page-level view of the link graph: edges are page-link BlockReferences
/// collapsed from (block -> page) to (the block's own page -> target page).
/// Layout (force-directed positioning) is a frontend concern — see
/// features/graph/components/graph-canvas (Cytoscape.js/D3), Fase 2 item #11.
/// </summary>
public class GetGraphDataHandler(
    IApplicationDbContext db,
    IPageRepository pageRepository,
    IBlockRepository blockRepository) : IQueryHandler<GetGraphDataQuery, GraphDataDto>
{
    public async Task<GraphDataDto> Handle(GetGraphDataQuery query, CancellationToken cancellationToken)
    {
        var pages = await pageRepository.ListAsync(query.WorkspaceId, cancellationToken);
        var pageIds = pages.Select(p => p.Id).ToHashSet();

        var references = await db.BlockReferences
            .Where(r => r.Type == BlockReferenceType.PageLink && r.TargetPageId != null && pageIds.Contains(r.TargetPageId.Value))
            .ToListAsync(cancellationToken);

        var sourceBlocks = await blockRepository.GetByIdsAsync(references.Select(r => r.SourceBlockId), cancellationToken);
        var pageIdByBlockId = sourceBlocks.ToDictionary(b => b.Id, b => b.PageId);

        var nodes = pages.Select(p => new GraphNodeDto(p.Id, p.Title.Value)).ToList();

        var edges = references
            .Where(r => pageIdByBlockId.ContainsKey(r.SourceBlockId))
            .Select(r => new GraphEdgeDto(pageIdByBlockId[r.SourceBlockId], r.TargetPageId!.Value))
            .Distinct()
            .ToList();

        return new GraphDataDto(nodes, edges);
    }
}
