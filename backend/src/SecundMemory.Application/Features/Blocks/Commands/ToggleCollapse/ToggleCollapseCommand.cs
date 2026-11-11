using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Dtos;

namespace SecundMemory.Application.Features.Blocks.Commands.ToggleCollapse;

public record ToggleCollapseCommand(Guid BlockId) : ICommand<BlockDto>;
