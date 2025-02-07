using CareerCompass.Application.Common;
using FluentValidation;

namespace CareerCompass.Application.Tags.Queries.GetTagByIdQuery;

public class GetTagByIdQueryValidator : AbstractValidator<GetTagByIdQuery>
{
    public GetTagByIdQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .IsGuid();

        RuleFor(x => x.TagId)
            .NotEmpty();

        RuleFor(x => x.TagId)
            .IsGuid();
    }
}