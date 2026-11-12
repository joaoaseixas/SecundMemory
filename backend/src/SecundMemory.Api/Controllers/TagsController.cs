using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Dtos;
using SecundMemory.Application.Features.Tags.Commands.ApplyTagToBlock;
using SecundMemory.Application.Features.Tags.Commands.CreateTag;
using SecundMemory.Application.Features.Tags.Commands.RemoveTagFromBlock;
using SecundMemory.Application.Features.Tags.Dtos;
using SecundMemory.Application.Features.Tags.Queries.GetBlocksByTag;
using SecundMemory.Application.Features.Tags.Queries.GetTagById;
using SecundMemory.Application.Features.Tags.Queries.ListTags;

namespace SecundMemory.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tags")]
public class TagsController(IDispatcher dispatcher, ICurrentUser currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TagDto>>> List(CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new ListTagsQuery(WorkspaceId), cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TagDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new GetTagByIdQuery(id), cancellationToken));

    [HttpGet("{id:guid}/blocks")]
    public async Task<ActionResult<IReadOnlyList<TaggedBlockDto>>> GetBlocks(Guid id, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new GetBlocksByTagQuery(WorkspaceId, id), cancellationToken));

    [HttpPost]
    public async Task<ActionResult<TagDto>> Create(CreateTagRequest request, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new CreateTagCommand(WorkspaceId, request.Name, request.PropertyTemplates), cancellationToken));

    [HttpPost("{tagId:guid}/blocks/{blockId:guid}")]
    public async Task<ActionResult<BlockDto>> ApplyToBlock(Guid tagId, Guid blockId, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new ApplyTagToBlockCommand(blockId, tagId), cancellationToken));

    [HttpDelete("{tagId:guid}/blocks/{blockId:guid}")]
    public async Task<ActionResult<BlockDto>> RemoveFromBlock(Guid tagId, Guid blockId, CancellationToken cancellationToken) =>
        Ok(await dispatcher.Send(new RemoveTagFromBlockCommand(blockId, tagId), cancellationToken));

    private Guid WorkspaceId => currentUser.WorkspaceId ?? throw new UnauthorizedAccessException("No workspace in context.");
}

public record CreateTagRequest(string Name, IReadOnlyList<TagPropertyTemplateDto> PropertyTemplates);
