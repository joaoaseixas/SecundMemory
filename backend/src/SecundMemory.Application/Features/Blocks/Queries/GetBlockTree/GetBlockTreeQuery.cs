using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Dtos;

namespace SecundMemory.Application.Features.Blocks.Queries.GetBlockTree;

public record GetBlockTreeQuery(Guid PageId) : IQuery<IReadOnlyList<BlockTreeNodeDto>>;
