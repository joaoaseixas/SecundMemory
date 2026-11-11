using Microsoft.EntityFrameworkCore;
using SecundMemory.Domain.Blocks;

namespace SecundMemory.Infrastructure.Persistence.Repositories;

public class BlockRepository(ApplicationDbContext db) : IBlockRepository
{
    public Task<Block?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Blocks.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Block>> GetByPageIdAsync(Guid pageId, CancellationToken cancellationToken = default) =>
        await db.Blocks.Where(b => b.PageId == pageId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Block>> GetChildrenAsync(Guid parentBlockId, CancellationToken cancellationToken = default) =>
        await db.Blocks.Where(b => b.ParentBlockId == parentBlockId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Block>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return [];

        return await db.Blocks.Where(b => idList.Contains(b.Id)).ToListAsync(cancellationToken);
    }

    public void Add(Block block) => db.Blocks.Add(block);

    public void Remove(Block block) => db.Blocks.Remove(block);
}
