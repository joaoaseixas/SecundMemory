using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Dtos;

namespace SecundMemory.Application.Features.Blocks.Commands.UpdateBlockProperty;

public record UpdateBlockPropertyCommand(Guid BlockId, string Key, string Value) : ICommand<BlockPropertyDto>;
