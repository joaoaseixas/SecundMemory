using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Attachments.Dtos;

namespace SecundMemory.Application.Features.Attachments.Queries.ListAttachments;

public record ListAttachmentsQuery(Guid? BlockId, Guid WorkspaceId) : IQuery<IReadOnlyList<AttachmentDto>>;
