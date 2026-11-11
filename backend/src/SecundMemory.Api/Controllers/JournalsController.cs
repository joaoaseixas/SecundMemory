using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Journals.Commands.OpenTodayJournal;
using SecundMemory.Application.Features.Journals.Dtos;
using SecundMemory.Application.Features.Journals.Queries.GetJournalByDate;

namespace SecundMemory.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/journals")]
public class JournalsController(IDispatcher dispatcher, ICurrentUser currentUser) : ControllerBase
{
    [HttpPost("today")]
    public async Task<ActionResult<JournalDto>> OpenToday(CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new OpenTodayJournalCommand(WorkspaceId), cancellationToken));

    [HttpGet("{date}")]
    public async Task<ActionResult<JournalDto>> GetByDate(DateOnly date, CancellationToken cancellationToken)
    {
        var journal = await dispatcher.Send(new GetJournalByDateQuery(WorkspaceId, date), cancellationToken);
        return journal is null ? NotFound() : Ok(journal);
    }

    private Guid WorkspaceId => currentUser.WorkspaceId ?? throw new UnauthorizedAccessException("No workspace in context.");
}
