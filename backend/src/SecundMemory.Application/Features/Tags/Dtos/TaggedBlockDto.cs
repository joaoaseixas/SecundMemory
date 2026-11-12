using SecundMemory.Application.Features.Blocks.Dtos;

namespace SecundMemory.Application.Features.Tags.Dtos;

public record TaggedBlockDto(Guid BlockId, Guid PageId, string PageTitle, string Content, IReadOnlyList<BlockPropertyDto> Properties);
