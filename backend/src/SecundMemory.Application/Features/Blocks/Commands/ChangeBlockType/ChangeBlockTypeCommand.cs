using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Dtos;
using SecundMemory.Domain.Blocks;

namespace SecundMemory.Application.Features.Blocks.Commands.ChangeBlockType;

public record ChangeBlockTypeCommand(Guid BlockId, BlockType Type) : ICommand<BlockDto>;
