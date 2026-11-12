using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Application.Features.Blocks.Dtos;
using SecundMemory.Domain.Properties;

namespace SecundMemory.Application.Features.Blocks.Commands.UpdateBlockProperty;

/// <summary>
/// Only updates an existing BlockProperty — the schema (which keys a block
/// can have) comes from the Tag(s) applied to it via ApplyTagToBlock, not
/// from editing a value here.
/// </summary>
public class UpdateBlockPropertyHandler(IApplicationDbContext db) : ICommandHandler<UpdateBlockPropertyCommand, BlockPropertyDto>
{
    public async Task<BlockPropertyDto> Handle(UpdateBlockPropertyCommand command, CancellationToken cancellationToken)
    {
        var property = await db.BlockProperties
            .FirstOrDefaultAsync(p => p.BlockId == command.BlockId && p.Key == command.Key, cancellationToken)
            ?? throw new NotFoundException(nameof(BlockProperty), command.Key);

        property.UpdateValue(command.Value);

        return new BlockPropertyDto(property.Key, property.Type, property.Value);
    }
}
