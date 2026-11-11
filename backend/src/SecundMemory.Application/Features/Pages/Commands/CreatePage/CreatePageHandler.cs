using FluentValidation.Results;
using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Pages.Dtos;
using SecundMemory.Domain.Pages;
using ValidationException = SecundMemory.Application.Common.Exceptions.ValidationException;

namespace SecundMemory.Application.Features.Pages.Commands.CreatePage;

public class CreatePageHandler(IPageRepository pageRepository) : ICommandHandler<CreatePageCommand, PageDto>
{
    public async Task<PageDto> Handle(CreatePageCommand command, CancellationToken cancellationToken)
    {
        var existing = await pageRepository.GetByTitleAsync(command.WorkspaceId, command.Title, cancellationToken);
        if (existing is not null)
            throw new ValidationException([new ValidationFailure(nameof(command.Title), "A page with this title already exists.")]);

        var page = Page.Create(command.WorkspaceId, PageTitle.Create(command.Title), command.IsJournal, command.JournalDate);
        pageRepository.Add(page);

        return page.ToDto();
    }
}
