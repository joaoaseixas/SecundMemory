using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Domain.Blocks;
using SecundMemory.Domain.Links;
using SecundMemory.Domain.Pages.Events;

namespace SecundMemory.Application.Features.Pages.EventHandlers;

/// <summary>
/// Keeps [[Old Title]] text inside other blocks' raw content in sync when a
/// page is renamed — the real reason Page+Block domain events exist, not
/// just a demonstration of the mechanism.
/// </summary>
public class PageRenamedEventHandler(
    IApplicationDbContext db,
    IBlockRepository blockRepository) : IDomainEventHandler<PageRenamedEvent>
{
    public async Task Handle(PageRenamedEvent domainEvent, CancellationToken cancellationToken)
    {
        var referencingBlockIds = await db.BlockReferences
            .Where(r => r.TargetPageId == domainEvent.PageId && r.Type == BlockReferenceType.PageLink)
            .Select(r => r.SourceBlockId)
            .ToListAsync(cancellationToken);

        if (referencingBlockIds.Count == 0) return;

        var oldLink = $"[[{domainEvent.OldTitle}]]";
        var newLink = $"[[{domainEvent.NewTitle}]]";

        var blocks = await blockRepository.GetByIdsAsync(referencingBlockIds, cancellationToken);
        foreach (var block in blocks.Where(b => b.Content.Value.Contains(oldLink)))
        {
            block.UpdateContent(BlockContent.Create(block.Content.Value.Replace(oldLink, newLink)));
        }
    }
}
