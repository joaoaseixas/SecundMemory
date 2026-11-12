using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Application.Features.Blocks.Common;
using SecundMemory.Application.Features.Blocks.Dtos;
using SecundMemory.Domain.Blocks;

namespace SecundMemory.Application.Features.Tags.Commands.RemoveTagFromBlock;

/// <summary>
/// Removing a tag only detaches it from the block — any BlockProperty rows
/// it previously seeded are left as-is. The user may have since filled them
/// with real data, and an untagging shouldn't silently discard that.
/// </summary>
public class RemoveTagFromBlockHandler(IBlockRepository blockRepository, IApplicationDbContext db)
    : ICommandHandler<RemoveTagFromBlockCommand, BlockDto>
{
    public async Task<BlockDto> Handle(RemoveTagFromBlockCommand command, CancellationToken cancellationToken)
    {
        var block = await blockRepository.GetByIdAsync(command.BlockId, cancellationToken)
            ?? throw new NotFoundException(nameof(Block), command.BlockId);

        block.RemoveTag(command.TagId);

        return block.ToDto(await BlockPropertyLoader.LoadAsync(db, block.Id, cancellationToken));
    }
}
