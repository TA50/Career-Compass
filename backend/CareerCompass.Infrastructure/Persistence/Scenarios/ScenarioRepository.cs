using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Infrastructure.Persistence.Scenarios;

internal class ScenarioRepository(AppDbContext dbContext) : IScenarioRepository
{
    public async Task<ErrorOr<Scenario>> Create(Scenario scenario, CancellationToken cancellationToken)
    {
        var agent = await dbContext.Agents.FirstAsync(a => a.Id == scenario.UserId, cancellationToken);

        var scenarioFields = new List<ScenarioFieldTable>();

        foreach (var scenarioField in scenario.ScenarioFields)
        {
            var field = await dbContext.Fields.FirstAsync(f => f.Id == scenarioField.FieldId, cancellationToken);
            var scenarioFieldTable = new ScenarioFieldTable
            {
                Id = Guid.CreateVersion7(),
                Field = field,
                Value = scenarioField.Value
            };
            scenarioFields.Add(scenarioFieldTable);
        }

        var tagIds = scenario.TagIds.Select(
            t => t.Value
        );

        var tags = await dbContext.Tags.Where(
            t => tagIds.Contains(t.Id)
        ).ToListAsync(cancellationToken);

        var scenarioTable = new ScenarioTable
        {
            Id = scenario.Id,
            Title = scenario.Title,
            Agent = agent,
            Date = scenario.Date,
            Tags = tags,
            ScenarioFields = scenarioFields
        };

        var result = await dbContext.Scenarios.AddAsync(scenarioTable, cancellationToken);


        await dbContext.SaveChangesAsync(cancellationToken);
        return MapScenario(result.Entity);
    }

    public Task<bool> Exists(ScenarioId id, CancellationToken cancellationToken)
    {
        return dbContext.Scenarios
            .AsNoTracking()
            .AnyAsync(f => f.Id == id, cancellationToken);
    }


    public async Task<Scenario> Get(ScenarioId id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Scenarios
            .AsNoTracking()
            .FirstAsync(f => f.Id == id, cancellationToken);

        return MapScenario(entity);
    }


    private Scenario MapScenario(ScenarioTable scenarioTable)
    {
        return new Scenario(
            id: scenarioTable.Id,
            title: scenarioTable.Title,
            tagIds: scenarioTable.Tags.Select(t => new TagId(t.Id)).ToList(),
            scenarioFields: scenarioTable.ScenarioFields.Select(f => new ScenarioField(f.Field.Id, f.Value)).ToList(),
            userId: new UserId(scenarioTable.Agent.Id),
            date: scenarioTable.Date
        );
    }
}