using ErrorOr;

namespace CareerCompass.Application.Scenarios;

public interface IScenarioRepository
{
    public Task<ErrorOr<Scenario>> Create(Scenario scenario, CancellationToken cancellationToken);

    public Task<bool> Exists(ScenarioId id, CancellationToken cancellationToken);
    // public Task<Scenario> GetById(ScenarioId id);
    //
    // public Task<Scenario> Update(Scenario scenario);
    //
    // public Task<Scenario> Delete(ScenarioId id);
}