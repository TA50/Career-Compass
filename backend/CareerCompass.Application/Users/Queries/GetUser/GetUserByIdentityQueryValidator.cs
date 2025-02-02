using FluentValidation;

namespace CareerCompass.Application.Users.Queries.GetUser;

public class GetUserByIdentityQueryValidator : AbstractValidator<GetUserByIdentityIdQuery>
{
    public GetUserByIdentityQueryValidator()
    {
        RuleFor(x => x.IdentityId)
            .NotEmpty();
    }
}