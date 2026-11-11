using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecundMemory.Domain.Links;
using SecundMemory.Infrastructure.Persistence;

namespace SecundMemory.Infrastructure.Search;

/// <summary>
/// Periodic consistency sweep, not the primary indexing path — that's the
/// real-time Application.Features.Blocks.EventHandlers.BlockContentIndexingEventHandler.
/// This just catches what event-driven indexing can't: e.g. a block embed
/// ((id)) left pointing at a block that was later hard-deleted elsewhere.
/// </summary>
public class SearchIndexUpdater(IServiceScopeFactory scopeFactory, ILogger<SearchIndexUpdater> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ReconcileOrphanedBlockEmbedsAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "SearchIndexUpdater sweep failed");
            }

            try
            {
                await Task.Delay(Interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task ReconcileOrphanedBlockEmbedsAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var blockEmbedRefs = await db.BlockReferences
            .Where(r => r.Type == BlockReferenceType.BlockEmbed)
            .ToListAsync(cancellationToken);

        if (blockEmbedRefs.Count == 0) return;

        var targetIds = blockEmbedRefs.Select(r => r.TargetBlockId!.Value).Distinct().ToList();
        var existingIds = (await db.Blocks.Where(b => targetIds.Contains(b.Id)).Select(b => b.Id).ToListAsync(cancellationToken))
            .ToHashSet();

        var orphaned = blockEmbedRefs.Where(r => !existingIds.Contains(r.TargetBlockId!.Value)).ToList();
        if (orphaned.Count == 0) return;

        db.BlockReferences.RemoveRange(orphaned);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("SearchIndexUpdater removed {Count} orphaned block embed references", orphaned.Count);
    }
}
