using SecundMemory.Application.Common.Cqrs;

namespace SecundMemory.Application.Features.Blocks.Commands.DeleteBlock;

public record DeleteBlockCommand(Guid BlockId) : ICommand;
