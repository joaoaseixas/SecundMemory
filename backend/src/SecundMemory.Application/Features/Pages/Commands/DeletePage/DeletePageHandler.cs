using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Common.Exceptions;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Pages.Commands.DeletePage;

public class DeletePageHandler(IPageRepository pageRepository) : ICommandHandler<DeletePageCommand>
{
    public async Task<Unit> Handle(DeletePageCommand command, CancellationToken cancellationToken)
    {
        var page = await pageRepository.GetByIdAsync(command.PageId, cancellationToken)
            ?? throw new NotFoundException(nameof(Page), command.PageId);

        page.MarkDeleted();

        return Unit.Value;
    }
}
