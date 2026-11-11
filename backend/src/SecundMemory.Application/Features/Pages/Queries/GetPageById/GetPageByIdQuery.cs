using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Pages.Dtos;

namespace SecundMemory.Application.Features.Pages.Queries.GetPageById;

public record GetPageByIdQuery(Guid PageId) : IQuery<PageDto>;
