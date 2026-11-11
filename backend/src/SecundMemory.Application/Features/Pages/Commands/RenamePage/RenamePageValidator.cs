using FluentValidation;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Pages.Commands.RenamePage;

public class RenamePageValidator : AbstractValidator<RenamePageCommand>
{
    public RenamePageValidator()
    {
        RuleFor(c => c.NewTitle).NotEmpty().MaximumLength(PageTitle.MaxLength);
    }
}
