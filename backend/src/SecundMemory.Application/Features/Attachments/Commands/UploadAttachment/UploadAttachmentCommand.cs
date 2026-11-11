using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Attachments.Dtos;

namespace SecundMemory.Application.Features.Attachments.Commands.UploadAttachment;

/// <remarks>Content is not disposed here — the caller (the Api controller, which owns the request's file stream) is responsible.</remarks>
public record UploadAttachmentCommand(Guid WorkspaceId, string FileName, string ContentType, Stream Content, Guid? BlockId = null)
    : ICommand<AttachmentDto>;
