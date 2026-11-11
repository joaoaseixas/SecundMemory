using Microsoft.EntityFrameworkCore;
using SecundMemory.Domain.Links;
using SecundMemory.Domain.Pages;
using SecundMemory.Infrastructure.Persistence;

namespace SecundMemory.Infrastructure.BackgroundJobs;

/// <summary>
/// On-demand full rebuild of a workspace's BlockReferences — not an
/// IHostedService; invoked explicitly (e.g. after a bulk Markdown import,
/// Fase 1.7) rather than running continuously like SearchIndexUpdater.
/// </summary>
public class ReindexSearchJob(ApplicationDbContext db)
{
    public async Task RunAsync(Guid workspaceId, CancellationToken cancellationToken = default)
    {
        var pages = await db.Pages.Where(p => p.WorkspaceId == workspaceId).ToListAsync(cancellationToken);
        var pageIds = pages.Select(p => p.Id).ToHashSet();
        var blocks = await db.Blocks.Where(b => pageIds.Contains(b.PageId)).ToListAsync(cancellationToken);
        var blockIds = blocks.Select(b => b.Id).ToList();

        var existingRefs = await db.BlockReferences.Where(r => blockIds.Contains(r.SourceBlockId)).ToListAsync(cancellationToken);
        db.BlockReferences.RemoveRange(existingRefs);

        foreach (var block in blocks)
        {
            foreach (var title in LinkParser.ExtractPageLinks(block.Content.Value))
            {
                var targetPage = pages.FirstOrDefault(p => p.Title.Value.Equals(title, StringComparison.OrdinalIgnoreCase));
                if (targetPage is null)
                {
                    targetPage = Page.Create(workspaceId, PageTitle.Create(title));
                    db.Pages.Add(targetPage);
                    pages.Add(targetPage);
                }

                db.BlockReferences.Add(BlockReference.ToPage(block.Id, targetPage.Id));
            }

            foreach (var targetBlockId in LinkParser.ExtractBlockReferences(block.Content.Value))
            {
                db.BlockReferences.Add(BlockReference.ToBlock(block.Id, targetBlockId));
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
