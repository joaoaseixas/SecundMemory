using FluentValidation;

namespace SecundMemory.Application.Features.Auth.Commands.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(c => c.Email).NotEmpty();
        RuleFor(c => c.DisplayName).NotEmpty().MaximumLength(128);
        RuleFor(c => c.Password).NotEmpty().MinimumLength(8);
    }
}
