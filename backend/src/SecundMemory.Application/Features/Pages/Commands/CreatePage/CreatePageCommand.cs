using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Pages.Dtos;

namespace SecundMemory.Application.Features.Pages.Commands.CreatePage;

public record CreatePageCommand(Guid WorkspaceId, string Title, bool IsJournal = false, DateOnly? JournalDate = null)
    : ICommand<PageDto>;
