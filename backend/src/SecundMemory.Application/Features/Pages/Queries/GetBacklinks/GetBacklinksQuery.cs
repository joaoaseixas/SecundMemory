using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Pages.Dtos;

namespace SecundMemory.Application.Features.Pages.Queries.GetBacklinks;

public record GetBacklinksQuery(Guid PageId) : IQuery<IReadOnlyList<BacklinkDto>>;
