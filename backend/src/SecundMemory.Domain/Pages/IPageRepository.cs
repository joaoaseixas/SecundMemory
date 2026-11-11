namespace SecundMemory.Domain.Pages;

public interface IPageRepository
{
    Task<Page?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Page?> GetByTitleAsync(Guid workspaceId, string title, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Page>> ListAsync(Guid workspaceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Page>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    void Add(Page page);
    void Remove(Page page);
}
