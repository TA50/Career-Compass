using FluentValidation;

namespace CareerCompass.Core.Tags.Commands.Delete;

public class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}