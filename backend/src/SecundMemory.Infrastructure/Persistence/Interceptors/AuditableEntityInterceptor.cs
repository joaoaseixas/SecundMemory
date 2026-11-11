using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Domain.Common;

namespace SecundMemory.Infrastructure.Persistence.Interceptors;

public class AuditableEntityInterceptor(ICurrentUser currentUser, IDateTimeProvider dateTimeProvider) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context is null) return;

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = dateTimeProvider.UtcNow;
                entry.Entity.CreatedBy = currentUser.UserId;
            }

            if (entry.State is EntityState.Added or EntityState.Modified || HasChangedOwnedEntities(entry))
            {
                entry.Entity.LastModifiedAtUtc = dateTimeProvider.UtcNow;
                entry.Entity.LastModifiedBy = currentUser.UserId;
            }
        }
    }

    private static bool HasChangedOwnedEntities(EntityEntry entry) =>
        entry.References.Any(r => r.TargetEntry is { } targetEntry &&
            targetEntry.Metadata.IsOwned() &&
            targetEntry.State is EntityState.Added or EntityState.Modified);
}
