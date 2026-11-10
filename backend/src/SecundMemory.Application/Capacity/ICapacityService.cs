namespace SecundMemory.Application.Capacity;

public interface ICapacityService
{
    Task<CapacitySummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
