using FluentValidation;

namespace SecundMemory.Application.Features.Workspaces.Commands.CreateWorkspace;

public class CreateWorkspaceValidator : AbstractValidator<CreateWorkspaceCommand>
{
    public CreateWorkspaceValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(128);
    }
}
