using CareerCompass.Application.Fields;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Scenarios.UseCases;
using CareerCompass.Application.Scenarios.UseCases.Contracts;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using ErrorOr;
using Moq;

namespace CareerCompass.Tests.Unit.Application.Scenarios;

/**
     * Given a scenario input
     * When Input Tag exists, and Fields exist
     * Then Scenario is created
     */
public class CreateScenarioTests
{
    private readonly List<Scenario> _scenarios = [];

    /**
     * Input is Valid
     */
    [Fact]
    public void GivenValidScenarioInput_ThenScenarioIsCreated()
    {
    }

    /**
     * Input is Invalid (Tag does not exist)
     */
    [Fact]
    public void GivenInvalidScenarioInputWithNonExistentTag_ThenScenarioIsNotCreatedAndErrorIsReturned()
    {
    }

    /**
     * Input is Invalid (Field does not exist)
     */
    [Fact]
    public void GivenInvalidScenarioInputWithNonExistentField_ThenScenarioIsNotCreatedAndErrorIsReturned()
    {
    }
}