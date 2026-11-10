using Microsoft.AspNetCore.Mvc;
using SecundMemory.Application.ProfileFacts;

namespace SecundMemory.Api.Controllers;

[ApiController]
[Route("api/profile-facts")]
public class ProfileFactsController(IProfileFactService profileFactService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProfileFactDto>>> GetAll([FromQuery] string? category, CancellationToken cancellationToken)
        => Ok(await profileFactService.GetAllAsync(category, cancellationToken));

    [HttpPut]
    public async Task<ActionResult<ProfileFactDto>> Upsert(UpsertProfileFactRequest request, CancellationToken cancellationToken)
        => Ok(await profileFactService.UpsertAsync(request, cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await profileFactService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
