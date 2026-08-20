using DiagramManager.Application.Constants;
using DiagramManager.Application.DTOs.Request;
using FluentValidation;

namespace DiagramManager.Application.Validators.Workspace;

public class CreateWorkspaceRequestValidator : AbstractValidator<CreateWorkspaceRequestDto>
{
    public CreateWorkspaceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(DiagramMessages.InvalidWorkspaceName)
            .Length(2, 200)
            .WithMessage(DiagramMessages.InvalidWorkspaceName);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(DiagramMessages.InvalidUserId);
    }
}
