using SecundMemory.Domain.Attachments;

namespace SecundMemory.Application.Features.Attachments.Dtos;

public record AttachmentDto(Guid Id, Guid WorkspaceId, Guid? BlockId, string FileName, string ContentType, long SizeBytes);

public static class AttachmentMappings
{
    public static AttachmentDto ToDto(this Attachment attachment) => new(
        attachment.Id, attachment.WorkspaceId, attachment.BlockId, attachment.FileName, attachment.ContentType, attachment.SizeBytes);
}
