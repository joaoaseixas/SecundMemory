using SecundMemory.Application.Common.Cqrs;

namespace SecundMemory.Application.Features.ImportExport.Commands.ImportMarkdown;

public record ImportMarkdownCommand(Guid WorkspaceId, string FileName, string MarkdownContent) : ICommand<Guid>;
