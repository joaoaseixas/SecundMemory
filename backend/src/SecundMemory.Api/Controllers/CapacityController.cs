using Microsoft.AspNetCore.Mvc;
using SecundMemory.Application.Capacity;

namespace SecundMemory.Api.Controllers;

[ApiController]
[Route("api/capacity")]
public class CapacityController(ICapacityService capacityService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CapacitySummaryDto>> Get(CancellationToken cancellationToken)
        => Ok(await capacityService.GetSummaryAsync(cancellationToken));
}
