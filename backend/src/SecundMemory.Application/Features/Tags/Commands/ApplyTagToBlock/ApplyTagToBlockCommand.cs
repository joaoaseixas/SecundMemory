using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Dtos;

namespace SecundMemory.Application.Features.Tags.Commands.ApplyTagToBlock;

public record ApplyTagToBlockCommand(Guid BlockId, Guid TagId) : ICommand<BlockDto>;
