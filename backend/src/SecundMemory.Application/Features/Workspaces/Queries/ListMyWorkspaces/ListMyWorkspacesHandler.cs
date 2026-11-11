using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Workspaces.Dtos;

namespace SecundMemory.Application.Features.Workspaces.Queries.ListMyWorkspaces;

public class ListMyWorkspacesHandler(IApplicationDbContext db) : IQueryHandler<ListMyWorkspacesQuery, IReadOnlyList<WorkspaceDto>>
{
    public async Task<IReadOnlyList<WorkspaceDto>> Handle(ListMyWorkspacesQuery query, CancellationToken cancellationToken)
    {
        var workspaces = await db.Workspaces
            .Where(w => w.Members.Any(m => m.UserId == query.UserId))
            .ToListAsync(cancellationToken);

        return workspaces.Select(w => w.ToDto()).ToList();
    }
}
