namespace SecundMemory.Application.Common.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}
