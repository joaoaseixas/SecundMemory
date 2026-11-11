using SecundMemory.Domain.Properties;
using SecundMemory.Domain.Tags;

namespace SecundMemory.Application.Features.Tags.Dtos;

public record TagPropertyTemplateDto(string Key, PropertyType Type, string? DefaultValue, bool AllowMultiple);

public record TagDto(Guid Id, Guid WorkspaceId, string Name, IReadOnlyList<TagPropertyTemplateDto> PropertyTemplates);

public static class TagMappings
{
    public static TagDto ToDto(this Tag tag) => new(
        tag.Id,
        tag.WorkspaceId,
        tag.Name,
        tag.PropertyTemplates.Select(t => new TagPropertyTemplateDto(t.Key, t.Type, t.DefaultValue, t.AllowMultiple)).ToList());
}
