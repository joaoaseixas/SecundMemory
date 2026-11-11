using SecundMemory.Application.Common.Cqrs;

namespace SecundMemory.Application.Features.ImportExport.Queries.ExportMarkdown;

public record ExportMarkdownQuery(Guid PageId) : IQuery<string>;
