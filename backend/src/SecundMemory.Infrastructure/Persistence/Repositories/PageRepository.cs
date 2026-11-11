using Microsoft.EntityFrameworkCore;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Infrastructure.Persistence.Repositories;

public class PageRepository(ApplicationDbContext db) : IPageRepository
{
    public Task<Page?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Pages.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<Page?> GetByTitleAsync(Guid workspaceId, string title, CancellationToken cancellationToken = default) =>
        db.Pages.FirstOrDefaultAsync(p => p.WorkspaceId == workspaceId && p.Title == PageTitle.Create(title), cancellationToken);

    public async Task<IReadOnlyList<Page>> ListAsync(Guid workspaceId, CancellationToken cancellationToken = default) =>
        await db.Pages.Where(p => p.WorkspaceId == workspaceId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Page>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return [];

        return await db.Pages.Where(p => idList.Contains(p.Id)).ToListAsync(cancellationToken);
    }

    public void Add(Page page) => db.Pages.Add(page);

    public void Remove(Page page) => db.Pages.Remove(page);
}
