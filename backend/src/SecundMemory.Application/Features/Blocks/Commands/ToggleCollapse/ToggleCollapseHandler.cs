using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Application.Features.Blocks.Dtos;
using SecundMemory.Domain.Blocks;

namespace SecundMemory.Application.Features.Blocks.Commands.ToggleCollapse;

public class ToggleCollapseHandler(IBlockRepository blockRepository) : ICommandHandler<ToggleCollapseCommand, BlockDto>
{
    public async Task<BlockDto> Handle(ToggleCollapseCommand command, CancellationToken cancellationToken)
    {
        var block = await blockRepository.GetByIdAsync(command.BlockId, cancellationToken)
            ?? throw new NotFoundException(nameof(Block), command.BlockId);

        block.ToggleCollapse();

        return block.ToDto();
    }
}
