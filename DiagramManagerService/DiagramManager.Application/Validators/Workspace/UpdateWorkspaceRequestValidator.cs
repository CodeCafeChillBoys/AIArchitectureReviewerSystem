using DiagramManager.Application.Constants;
using DiagramManager.Application.DTOs.Request;
using FluentValidation;

namespace DiagramManager.Application.Validators.Workspace;

public class UpdateWorkspaceRequestValidator : AbstractValidator<UpdateWorkspaceRequestDto>
{
    public UpdateWorkspaceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(DiagramMessages.InvalidWorkspaceName)
            .Length(2, 200)
            .WithMessage(DiagramMessages.InvalidWorkspaceName);
    }
}
