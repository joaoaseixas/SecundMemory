using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Dtos;
using SecundMemory.Domain.Blocks;

namespace SecundMemory.Application.Features.Blocks.Queries.GetBlockTree;

public class GetBlockTreeHandler(IBlockRepository blockRepository) : IQueryHandler<GetBlockTreeQuery, IReadOnlyList<BlockTreeNodeDto>>
{
    public async Task<IReadOnlyList<BlockTreeNodeDto>> Handle(GetBlockTreeQuery query, CancellationToken cancellationToken)
    {
        var blocks = await blockRepository.GetByPageIdAsync(query.PageId, cancellationToken);
        var byParent = blocks.ToLookup(b => b.ParentBlockId);

        return Build(null, byParent);
    }

    private static List<BlockTreeNodeDto> Build(Guid? parentId, ILookup<Guid?, Block> byParent) =>
        byParent[parentId]
            .OrderBy(b => b.Order)
            .Select(b => new BlockTreeNodeDto(b.ToDto(), Build(b.Id, byParent)))
            .ToList();
}
