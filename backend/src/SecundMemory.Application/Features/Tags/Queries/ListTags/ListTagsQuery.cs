using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Tags.Dtos;

namespace SecundMemory.Application.Features.Tags.Queries.ListTags;

public record ListTagsQuery(Guid WorkspaceId) : IQuery<IReadOnlyList<TagDto>>;
