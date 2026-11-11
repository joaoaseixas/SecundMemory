using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Pages.Dtos;

namespace SecundMemory.Application.Features.Pages.Commands.RenamePage;

public record RenamePageCommand(Guid PageId, string NewTitle) : ICommand<PageDto>;
