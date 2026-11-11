using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Application.Features.Blocks.Dtos;
using SecundMemory.Domain.Blocks;
using SecundMemory.Domain.Properties;
using SecundMemory.Domain.Tags;

namespace SecundMemory.Application.Features.Tags.Commands.ApplyTagToBlock;

/// <summary>
/// This is how "tags act as classes" (the original Fase 1 requirement)
/// actually happens: applying a Tag seeds a BlockProperty for each of its
/// PropertyTemplates that the block doesn't already have.
/// </summary>
public class ApplyTagToBlockHandler(IApplicationDbContext db, IBlockRepository blockRepository)
    : ICommandHandler<ApplyTagToBlockCommand, BlockDto>
{
    public async Task<BlockDto> Handle(ApplyTagToBlockCommand command, CancellationToken cancellationToken)
    {
        var block = await blockRepository.GetByIdAsync(command.BlockId, cancellationToken)
            ?? throw new NotFoundException(nameof(Block), command.BlockId);

        var tag = await db.Tags.FirstOrDefaultAsync(t => t.Id == command.TagId, cancellationToken)
            ?? throw new NotFoundException(nameof(Tag), command.TagId);

        block.AddTag(tag.Id);

        var existingKeys = await db.BlockProperties
            .Where(p => p.BlockId == block.Id)
            .Select(p => p.Key)
            .ToListAsync(cancellationToken);

        foreach (var template in tag.PropertyTemplates.Where(t => !existingKeys.Contains(t.Key)))
        {
            db.BlockProperties.Add(BlockProperty.Create(block.Id, template.Key, template.Type, template.DefaultValue ?? string.Empty));
        }

        return block.ToDto();
    }
}
