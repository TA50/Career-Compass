using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using CareerCompass.Infrastructure.Common;
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


        foreach (var sf in scenario.ScenarioFields)
        {
            await AddScenarioField(
                result.Entity,
                sf,
                cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);


        return await Get(new ScenarioId(result.Entity.Id), cancellationToken);
    }

    public async Task<Scenario> Get(ScenarioId id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Scenarios
            .AsNoTracking()
            .Include(s => s.Agent)
            .Include(s => s.ScenarioFields)
            .Include(s => s.Tags)
            .FirstOrDefaultAsync(f => f.Id.ToString() == id, cancellationToken);

        if (entity is null)
        {
            throw new EntityNotFoundException(id, nameof(Scenario));
        }

        return MapScenario(entity);
    }

    public Task<bool> Exists(ScenarioId id, CancellationToken cancellationToken)
    {
        return dbContext.Scenarios
            .AsNoTracking()
            .AnyAsync(f => f.Id.ToString() == id, cancellationToken);
    }

    public async Task<IList<Scenario>> Get(UserId userId, CancellationToken? cancellationToken = null)
    {
        var result = await dbContext
            .Scenarios
            .AsNoTracking()
            .Include(s => s.ScenarioFields)
            .Include(s => s.Agent)
            .Include(s => s.Tags)
            .Where(s => s.Agent.Id.ToString() == userId)
            .ToListAsync();


        return result.Select(MapScenario).ToList();
    }

    public Task<Scenario> Get(ScenarioId id, CancellationToken? cancellationToken = null)
    {
        return Get(Guid.Parse(id), cancellationToken ?? CancellationToken.None);
    }

    public async Task<Scenario> Update(Scenario scenario, CancellationToken? cancellationToken = null)
    {
        var entity = await dbContext
            .Scenarios
            .Include(s => s.Tags)
            .Include(s => s.ScenarioFields)
            .FirstOrDefaultAsync(x => x.Id.ToString() == scenario.Id.ToString(),
                cancellationToken ?? CancellationToken.None);

        if (entity is null)
        {
            throw new EntityNotFoundException(scenario.Id, nameof(Scenario));
        }


        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(cancellationToken ?? CancellationToken.None);
        try
        {
            entity.Date = scenario.Date;
            entity.Title = scenario.Title;


            // Step 1: Get current and incoming tag IDs
            var oldTagIds = entity.Tags.Select(t => t.Id.ToString()).ToList(); // Current tag IDs in the database
            var newTagIds =
                scenario.TagIds.Select(a => a.ToString()).ToList(); // Incoming tag IDs from the scenario

// Step 2: Determine which tags to add, remove, and update
            var tagsToAdd = newTagIds.Except(oldTagIds).ToList();
            var tagsToRemove = oldTagIds.Except(newTagIds).ToList();


            // Remove
            if (tagsToRemove.Any())
            {
                var tags = await dbContext.Tags
                    .Where(t => tagsToRemove.Contains(t.Id.ToString()))
                    .ToListAsync();

                foreach (var tag in tags)
                {
                    entity.Tags.Remove(tag);
                }
            }

            // Add
            if (tagsToAdd.Any())
            {
                var tags = await dbContext.Tags
                    .Where(t => tagsToAdd.Contains(t.Id.ToString()))
                    .ToListAsync();

                foreach (var tag in tags)
                {
                    entity.Tags.Add(tag);
                }
            }


            // Update Scenario Fields
            var oldFieldIds =
                entity.ScenarioFields.Select(t => t.FieldId.ToString()).ToList(); // Current tag IDs in the database
            var newFieldIds =
                scenario.ScenarioFields.Select(a => a.FieldId.ToString())
                    .ToList(); // Incoming tag IDs from the scenario
            // 
            // Step 2: Determine which scenarioFields to add, remove, and update
            var fieldsToAdd = newFieldIds.Except(oldFieldIds).ToList();
            var fieldsToRemove = oldFieldIds.Except(newFieldIds).ToList();
            var fieldsToUpdate = newFieldIds.Intersect(oldFieldIds).ToList();

            if (fieldsToAdd.Any())
            {
                foreach (var fieldId in fieldsToAdd)
                {
                    var scenarioField = scenario.ScenarioFields.First(sf => sf.FieldId.ToString() == fieldId);
                    await AddScenarioField(entity, scenarioField, cancellationToken);
                }
            }

            if (fieldsToRemove.Any())
            {
                foreach (var fieldId in fieldsToRemove)
                {
                    var scenarioField = entity.ScenarioFields.First(sf => sf.FieldId.ToString() == fieldId);
                    entity.ScenarioFields.Remove(scenarioField);
                }
            }

            if (fieldsToUpdate.Any())
            {
                foreach (var fieldId in fieldsToUpdate)
                {
                    var scenarioField = entity.ScenarioFields.First(sf => sf.FieldId.ToString() == fieldId);

                    var newScenarioFieldValue =
                        scenario.ScenarioFields.First(sf =>
                                sf.FieldId.ToString() == scenarioField.FieldId.ToString())
                            .Value;

                    // Update only if the value has changed (optional)
                    if (scenarioField.Value != newScenarioFieldValue)
                    {
                        scenarioField.Value = newScenarioFieldValue;
                    }
                }
            }


            await dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();

            throw;
        }

        return await Get(entity.Id, cancellationToken ?? CancellationToken.None);
    }

    private async Task AddScenarioField(
        ScenarioTable scenario,
        ScenarioField scenarioField,
        CancellationToken? cancellationToken = null
    )
    {
        var field = await dbContext.Fields.FirstAsync(f => f.Id.ToString() == scenarioField.FieldId.ToString(),
            cancellationToken ?? CancellationToken.None);


        var scenarioFieldTable = new ScenarioFieldTable
        {
            Id = Guid.CreateVersion7(),
            FieldId = field.Id,
            Value = scenarioField.Value,
            ScenarioId = scenario.Id
        };

        scenario.ScenarioFields.Add(scenarioFieldTable);
    }

    private Task<Scenario> Get(Guid id, CancellationToken cancellationToken)
    {
        return Get(new ScenarioId(id), cancellationToken);
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