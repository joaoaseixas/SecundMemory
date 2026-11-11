using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Dtos;
using SecundMemory.Domain.Blocks;

namespace SecundMemory.Application.Features.Blocks.Commands.CreateBlock;

public class CreateBlockHandler(IBlockRepository blockRepository) : ICommandHandler<CreateBlockCommand, BlockDto>
{
    public async Task<BlockDto> Handle(CreateBlockCommand command, CancellationToken cancellationToken)
    {
        var siblings = command.ParentBlockId is { } parentId
            ? await blockRepository.GetChildrenAsync(parentId, cancellationToken)
            : (await blockRepository.GetByPageIdAsync(command.PageId, cancellationToken))
                .Where(b => b.ParentBlockId is null)
                .ToList();

        var nextOrder = siblings.Count == 0 ? 1.0 : siblings.Max(b => b.Order) + 1.0;

        var block = Block.Create(command.PageId, command.ParentBlockId, BlockContent.Create(command.Content), nextOrder, command.Type);
        blockRepository.Add(block);

        return block.ToDto();
    }
}
