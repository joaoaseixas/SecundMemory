using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Application.Features.Blocks.Dtos;
using SecundMemory.Domain.Blocks;

namespace SecundMemory.Application.Features.Blocks.Commands.ChangeBlockType;

public class ChangeBlockTypeHandler(IBlockRepository blockRepository) : ICommandHandler<ChangeBlockTypeCommand, BlockDto>
{
    public async Task<BlockDto> Handle(ChangeBlockTypeCommand command, CancellationToken cancellationToken)
    {
        var block = await blockRepository.GetByIdAsync(command.BlockId, cancellationToken)
            ?? throw new NotFoundException(nameof(Block), command.BlockId);

        block.ChangeType(command.Type);

        return block.ToDto();
    }
}
