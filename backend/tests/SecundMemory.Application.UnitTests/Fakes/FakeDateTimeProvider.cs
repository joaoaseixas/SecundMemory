using SecundMemory.Application.Common.Abstractions;

namespace SecundMemory.Application.UnitTests.Fakes;

public class FakeDateTimeProvider(DateTime utcNow) : IDateTimeProvider
{
    public DateTime UtcNow { get; } = utcNow;
    public DateOnly Today { get; } = DateOnly.FromDateTime(utcNow);
}
