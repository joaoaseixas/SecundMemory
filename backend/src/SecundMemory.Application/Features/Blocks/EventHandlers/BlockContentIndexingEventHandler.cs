using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Domain.Blocks;
using SecundMemory.Domain.Blocks.Events;
using SecundMemory.Domain.Links;
using SecundMemory.Domain.Pages;
using SecundMemory.Domain.Tags;

namespace SecundMemory.Application.Features.Blocks.EventHandlers;

/// <summary>
/// Real-time link indexer: whenever a block's content is written, re-parses
/// it for [[Page Title]] links and ((block-id)) embeds, auto-creating any
/// page that doesn't exist yet (Fase 1 item #3), and rewrites this block's
/// BlockReference rows to match — which is what GetBacklinksQuery reads.
/// Infrastructure.Search.SearchIndexUpdater is a periodic consistency sweep
/// on top of this, not a replacement for it.
/// </summary>
public class BlockContentIndexingEventHandler(IApplicationDbContext db, IPageRepository pageRepository, IBlockRepository blockRepository) :
    IDomainEventHandler<BlockCreatedEvent>,
    IDomainEventHandler<BlockContentUpdatedEvent>
{
    public Task Handle(BlockCreatedEvent domainEvent, CancellationToken cancellationToken) =>
        ReindexAsync(domainEvent.BlockId, domainEvent.PageId, domainEvent.Content, cancellationToken);

    public Task Handle(BlockContentUpdatedEvent domainEvent, CancellationToken cancellationToken) =>
        ReindexAsync(domainEvent.BlockId, domainEvent.PageId, domainEvent.NewContent, cancellationToken);

    private async Task ReindexAsync(Guid blockId, Guid pageId, string? content, CancellationToken cancellationToken)
    {
        var existingRefs = await db.BlockReferences.Where(r => r.SourceBlockId == blockId).ToListAsync(cancellationToken);
        db.BlockReferences.RemoveRange(existingRefs);

        var sourcePage = await pageRepository.GetByIdAsync(pageId, cancellationToken);
        if (sourcePage is null) return;

        await SyncTagsAsync(blockId, sourcePage.WorkspaceId, content ?? string.Empty, cancellationToken);

        if (string.IsNullOrEmpty(content)) return;

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

    /// <summary>
    /// Makes the block's tags mirror the #tags in its text, creating Tag rows
    /// on first use — so the Tags screen fills up just by writing.
    /// </summary>
    private async Task SyncTagsAsync(Guid blockId, Guid workspaceId, string content, CancellationToken cancellationToken)
    {
        var block = await blockRepository.GetByIdAsync(blockId, cancellationToken);
        if (block is null) return;

        var names = LinkParser.ExtractTags(content);
        var wanted = new HashSet<Guid>();

        if (names.Count > 0)
        {
            var lowered = names.Select(n => n.ToLower()).ToList();
            var tags = await db.Tags
                .Where(t => t.WorkspaceId == workspaceId && lowered.Contains(t.Name.ToLower()))
                .ToListAsync(cancellationToken);

            foreach (var name in names)
            {
                var tag = tags.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    ?? db.Tags.Local.FirstOrDefault(t => t.WorkspaceId == workspaceId && t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (tag is null)
                {
                    tag = Tag.Create(workspaceId, name);
                    db.Tags.Add(tag);
                }
                wanted.Add(tag.Id);
            }
        }

        foreach (var stale in block.TagIds.Where(id => !wanted.Contains(id)).ToList()) block.RemoveTag(stale);
        foreach (var id in wanted) block.AddTag(id);
    }
}
