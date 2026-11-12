using Microsoft.EntityFrameworkCore;
using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Application.Features.Tags.Dtos;
using SecundMemory.Domain.Tags;

namespace SecundMemory.Application.Features.Tags.Queries.GetTagById;

public class GetTagByIdHandler(IApplicationDbContext db) : IQueryHandler<GetTagByIdQuery, TagDto>
{
    public async Task<TagDto> Handle(GetTagByIdQuery query, CancellationToken cancellationToken)
    {
        var tag = await db.Tags.FirstOrDefaultAsync(t => t.Id == query.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Tag), query.Id);

        return tag.ToDto();
    }
}
