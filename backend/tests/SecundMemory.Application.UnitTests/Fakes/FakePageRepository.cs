using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.UnitTests.Fakes;

public class FakePageRepository : IPageRepository
{
    private readonly Dictionary<Guid, Page> _pages = [];

    public Task<Page?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_pages.GetValueOrDefault(id));

    public Task<Page?> GetByTitleAsync(Guid workspaceId, string title, CancellationToken cancellationToken = default) =>
        Task.FromResult(_pages.Values.FirstOrDefault(p => p.WorkspaceId == workspaceId && p.Title.Value == title));

    public Task<IReadOnlyList<Page>> ListAsync(Guid workspaceId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Page>>(_pages.Values.Where(p => p.WorkspaceId == workspaceId).ToList());

    public Task<IReadOnlyList<Page>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idSet = ids.ToHashSet();
        return Task.FromResult<IReadOnlyList<Page>>(_pages.Values.Where(p => idSet.Contains(p.Id)).ToList());
    }

    public void Add(Page page) => _pages[page.Id] = page;

    public void Remove(Page page) => _pages.Remove(page.Id);
}
