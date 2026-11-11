using SecundMemory.Application.Common.Cqrs;

namespace SecundMemory.Application.Features.ImportExport.Queries.ExportMarkdown;

/// <summary>Stub per ADR 0004 — real Markdown export is Fase 1 item #7.</summary>
public class ExportMarkdownHandler : IQueryHandler<ExportMarkdownQuery, string>
{
    public Task<string> Handle(ExportMarkdownQuery query, CancellationToken cancellationToken) =>
        throw new NotImplementedException("Markdown export lands in Fase 1.7 — see docs/adr/0004.");
}
