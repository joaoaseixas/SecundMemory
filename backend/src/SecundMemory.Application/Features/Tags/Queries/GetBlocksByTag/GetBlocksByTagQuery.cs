using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Tags.Dtos;

namespace SecundMemory.Application.Features.Tags.Queries.GetBlocksByTag;

public record GetBlocksByTagQuery(Guid WorkspaceId, Guid TagId) : IQuery<IReadOnlyList<TaggedBlockDto>>;
