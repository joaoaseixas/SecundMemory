using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecundMemory.Api.Contracts.Blocks;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Commands.ChangeBlockType;
using SecundMemory.Application.Features.Blocks.Commands.CreateBlock;
using SecundMemory.Application.Features.Blocks.Commands.DeleteBlock;
using SecundMemory.Application.Features.Blocks.Commands.MoveBlock;
using SecundMemory.Application.Features.Blocks.Commands.ToggleCollapse;
using SecundMemory.Application.Features.Blocks.Commands.UpdateBlockContent;
using SecundMemory.Application.Features.Blocks.Dtos;
using SecundMemory.Application.Features.Blocks.Queries.GetBlockTree;

namespace SecundMemory.Api.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class BlocksController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet("pages/{pageId:guid}/blocks")]
    public async Task<ActionResult<IReadOnlyList<BlockTreeNodeDto>>> GetTree(Guid pageId, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new GetBlockTreeQuery(pageId), cancellationToken));

    [HttpPost("blocks")]
    public async Task<ActionResult<BlockDto>> Create(CreateBlockRequest request, CancellationToken cancellationToken)
    {
        var block = await dispatcher.Send(new CreateBlockCommand(request.PageId, request.ParentBlockId, request.Content, request.Type), cancellationToken);
        return CreatedAtAction(nameof(GetTree), new { pageId = block.PageId }, block);
    }

    [HttpPut("blocks/{id:guid}/content")]
    public async Task<ActionResult<BlockDto>> UpdateContent(Guid id, UpdateBlockContentRequest request, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new UpdateBlockContentCommand(id, request.Content), cancellationToken));

    [HttpPut("blocks/{id:guid}/move")]
    public async Task<ActionResult<BlockDto>> Move(Guid id, MoveBlockRequest request, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new MoveBlockCommand(id, request.NewParentBlockId, request.NewOrder), cancellationToken));

    [HttpPut("blocks/{id:guid}/collapse")]
    public async Task<ActionResult<BlockDto>> ToggleCollapse(Guid id, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new ToggleCollapseCommand(id), cancellationToken));

    [HttpPut("blocks/{id:guid}/type")]
    public async Task<ActionResult<BlockDto>> ChangeType(Guid id, ChangeBlockTypeRequest request, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new ChangeBlockTypeCommand(id, request.Type), cancellationToken));

    [HttpDelete("blocks/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await dispatcher.Send(new DeleteBlockCommand(id), cancellationToken);
        return NoContent();
    }
}
