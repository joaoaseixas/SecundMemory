using SecundMemory.Application.Common.Abstractions;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Tags.Dtos;
using SecundMemory.Domain.Tags;

namespace SecundMemory.Application.Features.Tags.Commands.CreateTag;

public class CreateTagHandler(IApplicationDbContext db) : ICommandHandler<CreateTagCommand, TagDto>
{
    public Task<TagDto> Handle(CreateTagCommand command, CancellationToken cancellationToken)
    {
        var tag = Tag.Create(command.WorkspaceId, command.Name);

        foreach (var template in command.PropertyTemplates)
        {
            tag.AddPropertyTemplate(new TagPropertyTemplate(template.Key, template.Type, template.DefaultValue, template.AllowMultiple));
        }

        db.Tags.Add(tag);

        return Task.FromResult(tag.ToDto());
    }
}
