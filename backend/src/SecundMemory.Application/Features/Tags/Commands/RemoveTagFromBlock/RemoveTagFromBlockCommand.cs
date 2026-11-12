using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Dtos;

namespace SecundMemory.Application.Features.Tags.Commands.RemoveTagFromBlock;

public record RemoveTagFromBlockCommand(Guid BlockId, Guid TagId) : ICommand<BlockDto>;
