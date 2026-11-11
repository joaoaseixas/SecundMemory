using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Application.Features.Pages.Dtos;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Pages.Commands.RenamePage;

public class RenamePageHandler(IPageRepository pageRepository) : ICommandHandler<RenamePageCommand, PageDto>
{
    public async Task<PageDto> Handle(RenamePageCommand command, CancellationToken cancellationToken)
    {
        var page = await pageRepository.GetByIdAsync(command.PageId, cancellationToken)
            ?? throw new NotFoundException(nameof(Page), command.PageId);

        page.Rename(PageTitle.Create(command.NewTitle));

        return page.ToDto();
    }
}
