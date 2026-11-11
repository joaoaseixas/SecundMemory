using FluentValidation;

namespace SecundMemory.Application.Features.Tags.Commands.CreateTag;

public class CreateTagValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(64);
    }
}
