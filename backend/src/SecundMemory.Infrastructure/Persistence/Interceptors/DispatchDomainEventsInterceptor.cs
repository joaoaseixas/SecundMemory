using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Domain.Common;

namespace SecundMemory.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Dispatches domain events BEFORE the actual save (not after) so that any
/// further entity changes a handler makes — e.g. PageRenamedEventHandler
/// rewriting other blocks' content — are swept into the very same
/// SaveChanges call rather than needing a second round-trip. Loops because a
/// handler's changes can themselves raise new events (cascades).
/// </summary>
public class DispatchDomainEventsInterceptor(IDomainEventDispatcher domainEventDispatcher) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await DispatchEventsAsync(eventData.Context, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchEventsAsync(DbContext context, CancellationToken cancellationToken)
    {
        List<IDomainEvent> events;

        do
        {
            var aggregatesWithEvents = context.ChangeTracker.Entries<AggregateRoot>()
                .Select(e => e.Entity)
                .Where(a => a.DomainEvents.Count > 0)
                .ToList();

            events = aggregatesWithEvents.SelectMany(a => a.DomainEvents).ToList();

            foreach (var aggregate in aggregatesWithEvents)
            {
                aggregate.ClearDomainEvents();
            }

            if (events.Count > 0)
            {
                await domainEventDispatcher.DispatchAsync(events, cancellationToken);
            }
        } while (events.Count > 0);
    }
}
