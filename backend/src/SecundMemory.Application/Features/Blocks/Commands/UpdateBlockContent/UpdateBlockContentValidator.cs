using FluentValidation;
using SecundMemory.Domain.Blocks;

namespace SecundMemory.Application.Features.Blocks.Commands.UpdateBlockContent;

public class UpdateBlockContentValidator : AbstractValidator<UpdateBlockContentCommand>
{
    public UpdateBlockContentValidator()
    {
        RuleFor(c => c.Content).MaximumLength(BlockContent.MaxLength);
    }
}
