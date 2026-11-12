using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Common;
using SecundMemory.Application.Features.Blocks.Dtos;
using SecundMemory.Domain.Blocks;

namespace SecundMemory.Application.Features.Blocks.Queries.GetBlockTree;

public class GetBlockTreeHandler(IBlockRepository blockRepository, IApplicationDbContext db) : IQueryHandler<GetBlockTreeQuery, IReadOnlyList<BlockTreeNodeDto>>
{
    public async Task<IReadOnlyList<BlockTreeNodeDto>> Handle(GetBlockTreeQuery query, CancellationToken cancellationToken)
    {
        var blocks = await blockRepository.GetByPageIdAsync(query.PageId, cancellationToken);
        var propertiesByBlock = await BlockPropertyLoader.LoadManyAsync(db, blocks.Select(b => b.Id).ToList(), cancellationToken);
        var byParent = blocks.ToLookup(b => b.ParentBlockId);

        return Build(null, byParent, propertiesByBlock);
    }

    private static List<BlockTreeNodeDto> Build(Guid? parentId, ILookup<Guid?, Block> byParent, ILookup<Guid, BlockPropertyDto> propertiesByBlock) =>
        byParent[parentId]
            .OrderBy(b => b.Order)
            .Select(b => new BlockTreeNodeDto(b.ToDto(propertiesByBlock[b.Id].ToList()), Build(b.Id, byParent, propertiesByBlock)))
            .ToList();
}
