using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Search.Queries.SearchAll;

namespace SecundMemory.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/search")]
public class SearchController(IDispatcher dispatcher, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SearchResult>>> Search([FromQuery] string q, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new SearchAllQuery(WorkspaceId, q), cancellationToken));

    private Guid WorkspaceId => currentUser.WorkspaceId ?? throw new UnauthorizedAccessException("No workspace in context.");
}
