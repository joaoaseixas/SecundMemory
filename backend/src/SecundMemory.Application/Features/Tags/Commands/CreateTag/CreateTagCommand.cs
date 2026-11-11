using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Tags.Dtos;

namespace SecundMemory.Application.Features.Tags.Commands.CreateTag;

public record CreateTagCommand(Guid WorkspaceId, string Name, IReadOnlyList<TagPropertyTemplateDto> PropertyTemplates)
    : ICommand<TagDto>;
