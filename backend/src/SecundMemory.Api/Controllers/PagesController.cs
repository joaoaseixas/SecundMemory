using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecundMemory.Api.Contracts.Pages;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Models;
using SecundMemory.Application.Features.Pages.Commands.CreatePage;
using SecundMemory.Application.Features.Pages.Commands.DeletePage;
using SecundMemory.Application.Features.Pages.Commands.RenamePage;
using SecundMemory.Application.Features.Pages.Dtos;
using SecundMemory.Application.Features.Pages.Queries.GetBacklinks;
using SecundMemory.Application.Features.Pages.Queries.GetPageById;
using SecundMemory.Application.Features.Pages.Queries.GetPageByTitle;
using SecundMemory.Application.Features.Pages.Queries.ListPages;

namespace SecundMemory.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/pages")]
public class PagesController(IDispatcher dispatcher, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<PageDto>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) =>
        Ok(await dispatcher.Send(new ListPagesQuery(WorkspaceId, new PaginationQuery(page, pageSize)), cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PageDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new GetPageByIdQuery(id), cancellationToken));

    /// <summary>
    /// Backs the editor's click-on-[[link]] navigation: resolve by exact
    /// title without the caller needing the id. Returns 200 with an empty
    /// body (not 404) when no such page exists yet — see GetPageByTitleHandler.
    /// </summary>
    [HttpGet("by-title")]
    public async Task<ActionResult<PageDto?>> GetByTitle([FromQuery] string title, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new GetPageByTitleQuery(WorkspaceId, title), cancellationToken));

    [HttpGet("{id:guid}/backlinks")]
    public async Task<ActionResult<IReadOnlyList<BacklinkDto>>> GetBacklinks(Guid id, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new GetBacklinksQuery(id), cancellationToken));

    [HttpPost]
    public async Task<ActionResult<PageDto>> Create(CreatePageRequest request, CancellationToken cancellationToken)
    {
        var page = await dispatcher.Send(new CreatePageCommand(WorkspaceId, request.Title, request.IsJournal, request.JournalDate), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = page.Id }, page);
    }

    [HttpPut("{id:guid}/title")]
    public async Task<ActionResult<PageDto>> Rename(Guid id, RenamePageRequest request, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new RenamePageCommand(id, request.NewTitle), cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await dispatcher.Send(new DeletePageCommand(id), cancellationToken);
        return NoContent();
    }

    private Guid WorkspaceId => currentUser.WorkspaceId ?? throw new UnauthorizedAccessException("No workspace in context.");
}
