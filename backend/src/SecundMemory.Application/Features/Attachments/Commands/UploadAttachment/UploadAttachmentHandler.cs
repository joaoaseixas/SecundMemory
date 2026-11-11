using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Attachments.Dtos;
using SecundMemory.Domain.Attachments;

namespace SecundMemory.Application.Features.Attachments.Commands.UploadAttachment;

public class UploadAttachmentHandler(IApplicationDbContext db, IFileStorage fileStorage)
    : ICommandHandler<UploadAttachmentCommand, AttachmentDto>
{
    public async Task<AttachmentDto> Handle(UploadAttachmentCommand command, CancellationToken cancellationToken)
    {
        var length = command.Content.CanSeek ? command.Content.Length : 0;
        var storageKey = await fileStorage.UploadAsync(command.FileName, command.Content, command.ContentType, cancellationToken);

        var attachment = Attachment.Create(command.WorkspaceId, command.FileName, command.ContentType, length, storageKey);
        if (command.BlockId is { } blockId) attachment.AttachToBlock(blockId);

        db.Attachments.Add(attachment);

        return attachment.ToDto();
    }
}
