using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecundMemory.Infrastructure.Persistence;

namespace SecundMemory.Infrastructure.BackgroundJobs;

/// <summary>Hard-deletes rows that have been soft-deleted longer than the retention period.</summary>
public class CleanupSoftDeletedJob(IServiceScopeFactory scopeFactory, ILogger<CleanupSoftDeletedJob> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(24);
    private static readonly TimeSpan RetentionPeriod = TimeSpan.FromDays(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PurgeAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "CleanupSoftDeletedJob sweep failed");
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

    private async Task PurgeAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var cutoff = DateTime.UtcNow - RetentionPeriod;

        var expiredPages = await db.Pages.IgnoreQueryFilters().Where(p => p.IsDeleted && p.DeletedAtUtc < cutoff).ToListAsync(cancellationToken);
        var expiredBlocks = await db.Blocks.IgnoreQueryFilters().Where(b => b.IsDeleted && b.DeletedAtUtc < cutoff).ToListAsync(cancellationToken);
        var expiredAttachments = await db.Attachments.IgnoreQueryFilters().Where(a => a.IsDeleted && a.DeletedAtUtc < cutoff).ToListAsync(cancellationToken);
        var expiredUsers = await db.Users.IgnoreQueryFilters().Where(u => u.IsDeleted && u.DeletedAtUtc < cutoff).ToListAsync(cancellationToken);

        var totalCount = expiredPages.Count + expiredBlocks.Count + expiredAttachments.Count + expiredUsers.Count;
        if (totalCount == 0) return;

        db.Pages.RemoveRange(expiredPages);
        db.Blocks.RemoveRange(expiredBlocks);
        db.Attachments.RemoveRange(expiredAttachments);
        db.Users.RemoveRange(expiredUsers);

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("CleanupSoftDeletedJob purged {Count} soft-deleted rows older than {Retention}", totalCount, RetentionPeriod);
    }
}
