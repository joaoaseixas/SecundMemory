using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Interfaces;
using SecundMemory.Domain.Entities;

namespace SecundMemory.Application.ProfileFacts;

public class ProfileFactService(IApplicationDbContext db) : IProfileFactService
{
    public async Task<IReadOnlyList<ProfileFactDto>> GetAllAsync(string? category, CancellationToken cancellationToken = default)
    {
        var facts = db.ProfileFacts.AsQueryable();
        if (!string.IsNullOrWhiteSpace(category))
        {
            facts = facts.Where(f => f.Category == category);
        }

        var results = await facts.OrderBy(f => f.Category).ThenBy(f => f.Key).ToListAsync(cancellationToken);
        return results.Select(ToDto).ToList();
    }

    public async Task<ProfileFactDto> UpsertAsync(UpsertProfileFactRequest request, CancellationToken cancellationToken = default)
    {
        var fact = await db.ProfileFacts.FirstOrDefaultAsync(
            f => f.Category == request.Category && f.Key == request.Key, cancellationToken);

        if (fact is null)
        {
            fact = new ProfileFact { Category = request.Category, Key = request.Key, Value = request.Value };
            db.ProfileFacts.Add(fact);
        }
        else
        {
            fact.Value = request.Value;
            fact.UpdatedUtc = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(cancellationToken);
        return ToDto(fact);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var fact = await db.ProfileFacts.FindAsync([id], cancellationToken);
        if (fact is null) return false;

        db.ProfileFacts.Remove(fact);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ProfileFactDto ToDto(ProfileFact fact) =>
        new(fact.Id, fact.Category, fact.Key, fact.Value, fact.UpdatedUtc);
}
