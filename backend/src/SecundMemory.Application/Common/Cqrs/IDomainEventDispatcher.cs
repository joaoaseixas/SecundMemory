using SecundMemory.Domain.Common;

namespace SecundMemory.Application.Common.Cqrs;

/// <summary>Called by Infrastructure's save-changes interceptor, never by handlers directly.</summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
