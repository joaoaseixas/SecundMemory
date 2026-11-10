using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Interfaces;

namespace SecundMemory.Application.Capacity;

public class CapacityService(IApplicationDbContext db) : ICapacityService
{
    public async Task<CapacitySummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var noteCount = await db.Notes.CountAsync(cancellationToken);
        var factCount = await db.ProfileFacts.CountAsync(cancellationToken);
        var tagCount = await db.Tags.CountAsync(cancellationToken);

        return new CapacitySummaryDto(noteCount, factCount, tagCount, DateTime.UtcNow);
    }
}
