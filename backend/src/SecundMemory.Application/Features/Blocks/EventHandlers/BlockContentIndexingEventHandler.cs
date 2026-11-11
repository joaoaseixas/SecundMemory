using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Domain.Blocks;
using SecundMemory.Domain.Blocks.Events;
using SecundMemory.Domain.Links;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Blocks.EventHandlers;

/// <summary>
/// Real-time link indexer: whenever a block's content is written, re-parses
/// it for [[Page Title]] links and ((block-id)) embeds, auto-creating any
/// page that doesn't exist yet (Fase 1 item #3), and rewrites this block's
/// BlockReference rows to match — which is what GetBacklinksQuery reads.
/// Infrastructure.Search.SearchIndexUpdater is a periodic consistency sweep
/// on top of this, not a replacement for it.
/// </summary>
public class BlockContentIndexingEventHandler(IApplicationDbContext db, IPageRepository pageRepository) :
    IDomainEventHandler<BlockCreatedEvent>,
    IDomainEventHandler<BlockContentUpdatedEvent>
{
    public Task Handle(BlockCreatedEvent domainEvent, CancellationToken cancellationToken) =>
        ReindexAsync(domainEvent.BlockId, domainEvent.PageId, cancellationToken);

    public Task Handle(BlockContentUpdatedEvent domainEvent, CancellationToken cancellationToken) =>
        ReindexAsync(domainEvent.BlockId, domainEvent.PageId, domainEvent.NewContent, cancellationToken);

    private async Task ReindexAsync(Guid blockId, Guid pageId, CancellationToken cancellationToken)
    {
        // BlockCreatedEvent doesn't carry content (a block is usually empty at
        // creation), but handle it uniformly in case a future caller creates
        // one with initial content already set.
        await ReindexAsync(blockId, pageId, null, cancellationToken);
    }

    private async Task ReindexAsync(Guid blockId, Guid pageId, string? content, CancellationToken cancellationToken)
    {
        var existingRefs = await db.BlockReferences.Where(r => r.SourceBlockId == blockId).ToListAsync(cancellationToken);
        db.BlockReferences.RemoveRange(existingRefs);

        if (string.IsNullOrEmpty(content)) return;

        var sourcePage = await pageRepository.GetByIdAsync(pageId, cancellationToken);
        if (sourcePage is null) return;

        foreach (var title in LinkParser.ExtractPageLinks(content))
        {
            var targetPage = await pageRepository.GetByTitleAsync(sourcePage.WorkspaceId, title, cancellationToken);
            if (targetPage is null)
            {
                targetPage = Page.Create(sourcePage.WorkspaceId, PageTitle.Create(title));
                pageRepository.Add(targetPage);
            }

            db.BlockReferences.Add(BlockReference.ToPage(blockId, targetPage.Id));
        }

        foreach (var targetBlockId in LinkParser.ExtractBlockReferences(content))
        {
            db.BlockReferences.Add(BlockReference.ToBlock(blockId, targetBlockId));
        }
    }
}
