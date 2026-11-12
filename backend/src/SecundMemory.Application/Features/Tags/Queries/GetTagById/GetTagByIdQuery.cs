using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Tags.Dtos;

namespace SecundMemory.Application.Features.Tags.Queries.GetTagById;

public record GetTagByIdQuery(Guid Id) : IQuery<TagDto>;
