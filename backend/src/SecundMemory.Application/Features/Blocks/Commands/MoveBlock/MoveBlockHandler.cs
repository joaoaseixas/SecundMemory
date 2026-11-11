using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Domain.Blocks;
using SecundMemory.Application.Features.Blocks.Dtos;

namespace SecundMemory.Application.Features.Blocks.Commands.MoveBlock;

public class MoveBlockHandler(IBlockRepository blockRepository) : ICommandHandler<MoveBlockCommand, BlockDto>
{
    public async Task<BlockDto> Handle(MoveBlockCommand command, CancellationToken cancellationToken)
    {
        var block = await blockRepository.GetByIdAsync(command.BlockId, cancellationToken)
            ?? throw new NotFoundException(nameof(Block), command.BlockId);

        if (command.NewParentBlockId == command.BlockId)
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(nameof(command.NewParentBlockId), "A block cannot be its own parent.")]);

        block.Move(command.NewParentBlockId, command.NewOrder);

        return block.ToDto();
    }
}
