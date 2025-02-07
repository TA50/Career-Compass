using CareerCompass.Application.Users;
using ErrorOr;

namespace CareerCompass.Application.Scenarios;

public interface IScenarioRepository
{
    public Task<ErrorOr<Scenario>> Create(Scenario scenario, CancellationToken cancellationToken);

    public Task<bool> Exists(ScenarioId id, CancellationToken cancellationToken);

    public Task<IList<Scenario>> Get(UserId userId, CancellationToken? cancellationToken = null);
    public Task<Scenario> Get(ScenarioId id, CancellationToken? cancellationToken = null);

    //
    public Task<Scenario> Update(Scenario scenario, CancellationToken? cancellationToken = null);
    //
    // public Task<Scenario> Delete(ScenarioId id);
}