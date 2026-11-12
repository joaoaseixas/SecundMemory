using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Features.Blocks.Dtos;

namespace SecundMemory.Application.Features.Blocks.Common;

/// <summary>
/// BlockProperty rows live outside the Block aggregate (seeded by applying a
/// Tag — see ApplyTagToBlockHandler), so every handler that returns a
/// BlockDto needs to batch-fetch them itself rather than reading them off
/// the Block entity. One shared lookup query avoids repeating that fetch
/// (and its N+1 risk for multi-block callers) in every handler.
/// </summary>
internal static class BlockPropertyLoader
{
    public static async Task<ILookup<Guid, BlockPropertyDto>> LoadManyAsync(
        IApplicationDbContext db, IReadOnlyCollection<Guid> blockIds, CancellationToken cancellationToken)
    {
        // EF Core translates .Contains over an empty collection to a predicate that matches
        // nothing, so this stays correct (and cheap) without a separate empty-input branch.
        var properties = await db.BlockProperties.Where(p => blockIds.Contains(p.BlockId)).ToListAsync(cancellationToken);
        return properties.ToLookup(p => p.BlockId, p => new BlockPropertyDto(p.Key, p.Type, p.Value));
    }

    public static async Task<IReadOnlyList<BlockPropertyDto>> LoadAsync(
        IApplicationDbContext db, Guid blockId, CancellationToken cancellationToken)
    {
        var byBlock = await LoadManyAsync(db, [blockId], cancellationToken);
        return byBlock[blockId].ToList();
    }
}
