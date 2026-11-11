using FluentValidation;
using SecundMemory.Domain.Blocks;

namespace SecundMemory.Application.Features.Blocks.Commands.CreateBlock;

public class CreateBlockValidator : AbstractValidator<CreateBlockCommand>
{
    public CreateBlockValidator()
    {
        RuleFor(c => c.Content).MaximumLength(BlockContent.MaxLength);
    }
}
