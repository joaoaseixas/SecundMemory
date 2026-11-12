using SecundMemory.Domain.Blocks;

namespace SecundMemory.Api.Contracts.Blocks;

public record CreateBlockRequest(Guid PageId, Guid? ParentBlockId, string Content, BlockType Type = BlockType.Text);
public record UpdateBlockContentRequest(string Content);
public record MoveBlockRequest(Guid? NewParentBlockId, double NewOrder);
public record ChangeBlockTypeRequest(BlockType Type);
public record UpdateBlockPropertyRequest(string Value);
