using FluentValidation;
using SecundMemory.Domain.Pages;

namespace SecundMemory.Application.Features.Pages.Commands.CreatePage;

public class CreatePageValidator : AbstractValidator<CreatePageCommand>
{
    public CreatePageValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(PageTitle.MaxLength);
    }
}
