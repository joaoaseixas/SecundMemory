using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Tags.Dtos;

namespace SecundMemory.Application.Features.Tags.Queries.ListTags;

public class ListTagsHandler(IApplicationDbContext db) : IQueryHandler<ListTagsQuery, IReadOnlyList<TagDto>>
{
    public async Task<IReadOnlyList<TagDto>> Handle(ListTagsQuery query, CancellationToken cancellationToken)
    {
        var tags = await db.Tags.Where(t => t.WorkspaceId == query.WorkspaceId).ToListAsync(cancellationToken);
        return tags.Select(t => t.ToDto()).ToList();
    }
}
