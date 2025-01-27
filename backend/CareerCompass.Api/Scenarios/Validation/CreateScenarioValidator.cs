using CareerCompass.Api.Scenarios.Contracts;
using FluentValidation;

namespace CareerCompass.Api.Scenarios.Validation;

public class CreateScenarioValidator : AbstractValidator<CreateScenarioDto>
{
    public CreateScenarioValidator()
    {
        RuleFor(dto => dto.UserId).NotNull();
        RuleFor(dto => dto.Title).NotNull();
    }
}