using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Domain.Blocks;

namespace SecundMemory.Application.Features.Blocks.Commands.DeleteBlock;

/// <summary>Deletes the block and its entire subtree — an outliner block owns its children.</summary>
public class DeleteBlockHandler(IBlockRepository blockRepository) : ICommandHandler<DeleteBlockCommand>
{
    public async Task<Unit> Handle(DeleteBlockCommand command, CancellationToken cancellationToken)
    {
        var block = await blockRepository.GetByIdAsync(command.BlockId, cancellationToken)
            ?? throw new NotFoundException(nameof(Block), command.BlockId);

        await DeleteRecursivelyAsync(block, cancellationToken);

        return Unit.Value;
    }

    private async Task DeleteRecursivelyAsync(Block block, CancellationToken cancellationToken)
    {
        var children = await blockRepository.GetChildrenAsync(block.Id, cancellationToken);
        foreach (var child in children)
        {
            await DeleteRecursivelyAsync(child, cancellationToken);
        }

        block.MarkDeleted();
    }
}
