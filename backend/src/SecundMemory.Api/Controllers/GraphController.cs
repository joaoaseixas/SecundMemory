using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Graph.Dtos;
using SecundMemory.Application.Features.Graph.Queries.GetGraphData;

namespace SecundMemory.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/graph")]
public class GraphController(IDispatcher dispatcher, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GraphDataDto>> Get(CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new GetGraphDataQuery(WorkspaceId), cancellationToken));

    private Guid WorkspaceId => currentUser.WorkspaceId ?? throw new UnauthorizedAccessException("No workspace in context.");
}
