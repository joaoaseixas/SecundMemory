using Microsoft.AspNetCore.Mvc;
using SecundMemory.Application.Notes;

namespace SecundMemory.Api.Controllers;

[ApiController]
[Route("api/notes")]
public class NotesController(INoteService noteService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NoteDto>>> Search([FromQuery] string? query, CancellationToken cancellationToken)
        => Ok(await noteService.SearchAsync(query, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<NoteDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var note = await noteService.GetByIdAsync(id, cancellationToken);
        return note is null ? NotFound() : Ok(note);
    }

    [HttpPost]
    public async Task<ActionResult<NoteDto>> Create(CreateNoteRequest request, CancellationToken cancellationToken)
    {
        var note = await noteService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<NoteDto>> Update(Guid id, UpdateNoteRequest request, CancellationToken cancellationToken)
    {
        var note = await noteService.UpdateAsync(id, request, cancellationToken);
        return note is null ? NotFound() : Ok(note);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await noteService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
