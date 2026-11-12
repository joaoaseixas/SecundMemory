namespace SecundMemory.Domain.Blocks;

public interface IBlockRepository
{
    Task<Block?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Block>> GetByPageIdAsync(Guid pageId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Block>> GetChildrenAsync(Guid parentBlockId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Block>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Block>> GetByTagIdAsync(Guid workspaceId, Guid tagId, CancellationToken cancellationToken = default);
    void Add(Block block);
    void Remove(Block block);
}
