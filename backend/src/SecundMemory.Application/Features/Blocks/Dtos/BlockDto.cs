using SecundMemory.Domain.Blocks;
using SecundMemory.Domain.Properties;

namespace SecundMemory.Application.Features.Blocks.Dtos;

public record BlockPropertyDto(string Key, PropertyType Type, string Value);

public record BlockDto(
    Guid Id,
    Guid PageId,
    Guid? ParentBlockId,
    string Content,
    BlockType Type,
    double Order,
    bool IsCollapsed,
    IReadOnlyList<Guid> TagIds,
    IReadOnlyList<BlockPropertyDto> Properties,
    DateTime CreatedAtUtc,
    DateTime? LastModifiedAtUtc);

public record BlockTreeNodeDto(BlockDto Block, IReadOnlyList<BlockTreeNodeDto> Children);

public static class BlockMappings
{
    public static BlockDto ToDto(this Block block, IReadOnlyList<BlockPropertyDto>? properties = null) => new(
        block.Id,
        block.PageId,
        block.ParentBlockId,
        block.Content.Value,
        block.Type,
        block.Order,
        block.IsCollapsed,
        block.TagIds.ToList(),
        properties ?? [],
        block.CreatedAtUtc,
        block.LastModifiedAtUtc);
}
