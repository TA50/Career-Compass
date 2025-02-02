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
        var agent = await dbContext.Agents.FirstAsync(a => a.Id.ToString() == scenario.UserId, cancellationToken);


        var tagIds = scenario.TagIds.Select(
            t => t.Value
        );

        var tags = await dbContext.Tags.Where(
            t => tagIds.Contains(t.Id.ToString())
        ).ToListAsync(cancellationToken);

        var scenarioTable = new ScenarioTable
        {
            Id = Guid.Parse(scenario.Id),
            Title = scenario.Title,
            Agent = agent,
            Date = scenario.Date,
            Tags = tags,
            ScenarioFields = []
        };

        var result = await dbContext.Scenarios.AddAsync(scenarioTable, cancellationToken);


        await dbContext.SaveChangesAsync(cancellationToken);

        if (!scenario.ScenarioFields.Any())
        {
            return MapScenario(result.Entity);
        }


        foreach (var scenarioField in scenario.ScenarioFields)
        {
            var field = await dbContext.Fields.FirstAsync(f => f.Id.ToString() == scenarioField.FieldId.ToString(),
                cancellationToken);
            var scenarioFieldTable = new ScenarioFieldTable
            {
                Id = Guid.CreateVersion7(),
                FieldId = field.Id,
                Value = scenarioField.Value,
                ScenarioId = result.Entity.Id
            };
            await dbContext.ScenarioFields.AddAsync(scenarioFieldTable, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);


        return await Get(new ScenarioId(result.Entity.Id), cancellationToken);
    }

    public Task<bool> Exists(ScenarioId id, CancellationToken cancellationToken)
    {
        return dbContext.Scenarios
            .AsNoTracking()
            .AnyAsync(f => f.Id.ToString() == id, cancellationToken);
    }


    public async Task<Scenario> Get(ScenarioId id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Scenarios
            .AsNoTracking()
            .Include(s => s.Agent)
            .Include(s => s.ScenarioFields)
            .Include(s => s.Tags)
            .FirstAsync(f => f.Id.ToString() == id, cancellationToken);

        return MapScenario(entity);
    }


    private Scenario MapScenario(ScenarioTable scenarioTable)
    {
        return new Scenario(
            id: scenarioTable.Id.ToString(),
            title: scenarioTable.Title,
            tagIds: scenarioTable.Tags.Select(t => new TagId(t.Id)).ToList(),
            scenarioFields: scenarioTable.ScenarioFields.Select(f => new ScenarioField(f.FieldId, f.Value)).ToList(),
            userId: new UserId(scenarioTable.Agent.Id),
            date: scenarioTable.Date
        );
    }
}