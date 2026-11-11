using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Blocks.Dtos;

namespace SecundMemory.Application.Features.Blocks.Commands.MoveBlock;

/// <summary>
/// Order is given explicitly by the caller (the frontend computes a
/// fractional value between the two blocks it's being dropped between — see
/// shared/utils/tree.utils.ts) rather than recomputed server-side, since only
/// the client knows exactly where in the visible list the drop happened.
/// </summary>
public record MoveBlockCommand(Guid BlockId, Guid? NewParentBlockId, double NewOrder) : ICommand<BlockDto>;
