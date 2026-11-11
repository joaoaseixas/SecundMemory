using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Graph.Dtos;

namespace SecundMemory.Application.Features.Graph.Queries.GetGraphData;

public record GetGraphDataQuery(Guid WorkspaceId) : IQuery<GraphDataDto>;
