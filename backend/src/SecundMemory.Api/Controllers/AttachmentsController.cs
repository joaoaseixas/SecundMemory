using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Attachments.Commands.UploadAttachment;
using SecundMemory.Application.Features.Attachments.Dtos;
using SecundMemory.Application.Features.Attachments.Queries.ListAttachments;

namespace SecundMemory.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/attachments")]
public class AttachmentsController(IDispatcher dispatcher, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AttachmentDto>>> List([FromQuery] Guid? blockId, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new ListAttachmentsQuery(blockId, WorkspaceId), cancellationToken));

    [HttpPost]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<AttachmentDto>> Upload(IFormFile file, [FromForm] Guid? blockId, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        var command = new UploadAttachmentCommand(WorkspaceId, file.FileName, file.ContentType, stream, blockId);
        return Ok(await dispatcher.Send(command, cancellationToken));
    }

    private Guid WorkspaceId => currentUser.WorkspaceId ?? throw new UnauthorizedAccessException("No workspace in context.");
}
