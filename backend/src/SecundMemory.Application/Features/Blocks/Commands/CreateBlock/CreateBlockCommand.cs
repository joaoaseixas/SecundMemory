using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Dtos;
using SecundMemory.Domain.Blocks;

namespace SecundMemory.Application.Features.Blocks.Commands.CreateBlock;

public record CreateBlockCommand(Guid PageId, Guid? ParentBlockId, string Content, BlockType Type = BlockType.Text)
    : ICommand<BlockDto>;
