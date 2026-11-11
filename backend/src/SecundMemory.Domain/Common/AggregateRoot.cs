namespace SecundMemory.Domain.Common;

/// <summary>
/// An Entity that is the single entry point for changes to a cluster of
/// objects (itself and anything it owns). Only aggregate roots raise domain
/// events — anything inside the cluster is reached and mutated through it.
/// Always auditable: every aggregate root tracks who created/modified it.
/// </summary>
public abstract class AggregateRoot : AuditableEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
