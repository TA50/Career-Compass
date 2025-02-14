using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Scenarios;

namespace CareerCompass.Infrastructure.Persistence.Repositories;

internal class ScenarioRepository(AppDbContext dbContext)
    : RepositoryBase<Scenario, ScenarioId>(dbContext), IScenarioRepository
{
}