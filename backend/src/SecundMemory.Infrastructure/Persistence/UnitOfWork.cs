using SecundMemory.Application.Common.Abstractions;

namespace SecundMemory.Infrastructure.Persistence;

public class UnitOfWork(ApplicationDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
