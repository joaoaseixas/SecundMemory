using SecundMemory.Application.Common.Cqrs;

namespace SecundMemory.Application.Features.Pages.Commands.DeletePage;

public record DeletePageCommand(Guid PageId) : ICommand;
