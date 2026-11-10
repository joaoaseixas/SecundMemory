using Microsoft.EntityFrameworkCore;
using SecundMemory.Domain.Entities;

namespace SecundMemory.Application.Common.Interfaces;

/// <summary>
/// Abstraction over the EF Core DbContext so Application services depend only
/// on this interface, never on Infrastructure/EF Core directly.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Note> Notes { get; }
    DbSet<Tag> Tags { get; }
    DbSet<ProfileFact> ProfileFacts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
