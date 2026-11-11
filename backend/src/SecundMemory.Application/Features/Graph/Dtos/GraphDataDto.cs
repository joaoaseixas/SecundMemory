namespace SecundMemory.Application.Features.Graph.Dtos;

public record GraphNodeDto(Guid PageId, string Title);
public record GraphEdgeDto(Guid SourcePageId, Guid TargetPageId);
public record GraphDataDto(IReadOnlyList<GraphNodeDto> Nodes, IReadOnlyList<GraphEdgeDto> Edges);
