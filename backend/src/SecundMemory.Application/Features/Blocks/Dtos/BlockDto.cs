using SecundMemory.Domain.Blocks;

namespace SecundMemory.Application.Features.Blocks.Dtos;

public record BlockDto(
    Guid Id,
    Guid PageId,
    Guid? ParentBlockId,
    string Content,
    BlockType Type,
    double Order,
    bool IsCollapsed,
    IReadOnlyList<Guid> TagIds,
    DateTime CreatedAtUtc,
    DateTime? LastModifiedAtUtc);

public record BlockTreeNodeDto(BlockDto Block, IReadOnlyList<BlockTreeNodeDto> Children);

public static class BlockMappings
{
    public static BlockDto ToDto(this Block block) => new(
        block.Id,
        block.PageId,
        block.ParentBlockId,
        block.Content.Value,
        block.Type,
        block.Order,
        block.IsCollapsed,
        block.TagIds.ToList(),
        block.CreatedAtUtc,
        block.LastModifiedAtUtc);
}
