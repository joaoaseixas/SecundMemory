using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Pages.Dtos;

namespace SecundMemory.Application.Features.Pages.Queries.GetPageByTitle;

public record GetPageByTitleQuery(Guid WorkspaceId, string Title) : IQuery<PageDto?>;
