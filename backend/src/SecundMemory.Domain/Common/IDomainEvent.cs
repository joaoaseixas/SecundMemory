namespace SecundMemory.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
