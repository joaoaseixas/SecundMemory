using SecundMemory.Domain.Blocks;

namespace SecundMemory.Application.UnitTests.Fakes;

public class FakeBlockRepository : IBlockRepository
{
    private readonly Dictionary<Guid, Block> _blocks = [];

    public Task<Block?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_blocks.GetValueOrDefault(id));

    public Task<IReadOnlyList<Block>> GetByPageIdAsync(Guid pageId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Block>>(_blocks.Values.Where(b => b.PageId == pageId).ToList());

    public Task<IReadOnlyList<Block>> GetChildrenAsync(Guid parentBlockId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Block>>(_blocks.Values.Where(b => b.ParentBlockId == parentBlockId).ToList());

    public Task<IReadOnlyList<Block>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idSet = ids.ToHashSet();
        return Task.FromResult<IReadOnlyList<Block>>(_blocks.Values.Where(b => idSet.Contains(b.Id)).ToList());
    }

    /// <summary>
    /// Ignores workspaceId: unlike the real BlockRepository, this fake has no
    /// Page/Workspace data to join against, and tests only ever add blocks
    /// relevant to the scenario under test, so tag-id filtering alone is
    /// enough here. Workspace scoping is covered by the real implementation
    /// via Infrastructure.IntegrationTests instead.
    /// </summary>
    public Task<IReadOnlyList<Block>> GetByTagIdAsync(Guid workspaceId, Guid tagId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Block>>(_blocks.Values.Where(b => b.TagIds.Contains(tagId)).ToList());

    public void Add(Block block) => _blocks[block.Id] = block;

    public void Remove(Block block) => _blocks.Remove(block.Id);
}
