using SecundMemory.Domain.Common;

namespace SecundMemory.Application.Common.Cqrs;

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken cancellationToken);
}
