using SecundMemory.Application.Common.Cqrs;

namespace SecundMemory.Application.Features.ImportExport.Commands.ImportMarkdown;

/// <summary>
/// Stub per ADR 0004 — real Markdown import (parsing Logseq-style outline
/// markdown into a Page+Block tree) is Fase 1 item #7, deliberately not part
/// of the foundation skeleton.
/// </summary>
public class ImportMarkdownHandler : ICommandHandler<ImportMarkdownCommand, Guid>
{
    public Task<Guid> Handle(ImportMarkdownCommand command, CancellationToken cancellationToken) =>
        throw new NotImplementedException("Markdown import lands in Fase 1.7 — see docs/adr/0004.");
}
