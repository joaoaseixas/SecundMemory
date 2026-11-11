using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Attachments.Dtos;

namespace SecundMemory.Application.Features.Attachments.Queries.ListAttachments;

public class ListAttachmentsHandler(IApplicationDbContext db) : IQueryHandler<ListAttachmentsQuery, IReadOnlyList<AttachmentDto>>
{
    public async Task<IReadOnlyList<AttachmentDto>> Handle(ListAttachmentsQuery query, CancellationToken cancellationToken)
    {
        var attachments = await db.Attachments
            .Where(a => a.WorkspaceId == query.WorkspaceId && (query.BlockId == null || a.BlockId == query.BlockId))
            .ToListAsync(cancellationToken);

        return attachments.Select(a => a.ToDto()).ToList();
    }
}
