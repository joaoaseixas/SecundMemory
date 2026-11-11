using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Workspaces.Dtos;

namespace SecundMemory.Application.Features.Workspaces.Queries.ListMyWorkspaces;

public record ListMyWorkspacesQuery(Guid UserId) : IQuery<IReadOnlyList<WorkspaceDto>>;
