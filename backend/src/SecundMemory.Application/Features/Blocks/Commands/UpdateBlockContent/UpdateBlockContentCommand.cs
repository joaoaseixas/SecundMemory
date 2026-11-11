using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Dtos;

namespace SecundMemory.Application.Features.Blocks.Commands.UpdateBlockContent;

public record UpdateBlockContentCommand(Guid BlockId, string Content) : ICommand<BlockDto>;
