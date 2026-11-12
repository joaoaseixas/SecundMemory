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

    /// <summary>
    /// TagIds is a jsonb list on Block, not a relational FK, so EF Core can't
    /// translate .Contains(tagId) against it into SQL (the same class of
    /// limitation as the Email/.Value issue elsewhere) — filter by workspace
    /// in SQL via the Pages join, then filter by tag in memory.
    /// </summary>
    public async Task<IReadOnlyList<Block>> GetByTagIdAsync(Guid workspaceId, Guid tagId, CancellationToken cancellationToken = default)
    {
        var pageIds = await db.Pages.Where(p => p.WorkspaceId == workspaceId).Select(p => p.Id).ToListAsync(cancellationToken);
        if (pageIds.Count == 0) return [];

        var blocks = await db.Blocks.Where(b => pageIds.Contains(b.PageId)).ToListAsync(cancellationToken);
        return blocks.Where(b => b.TagIds.Contains(tagId)).ToList();
    }

    public void Add(Block block) => db.Blocks.Add(block);

    public void Remove(Block block) => db.Blocks.Remove(block);
}
