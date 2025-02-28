using Bogus;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using CareerCompass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Tooling.Seeders;

class ScenarioSeeder(AppDbContext dbContext, ILoggerAdapter<ScenarioSeeder> logger) : IDatabaseSeeder
{
    private readonly Faker _faker = new();

    public async Task SeedAsync(int count = 1)
    {
        logger.LogInformation("Seeding {Count} scenarios...", count);
        var userIds = await dbContext.Users.Select(u => u.Id).ToListAsync();
        var tagIds = await dbContext.Tags.Select(t => t.Id).ToListAsync();
        var fieldIds = await dbContext.Fields.Select(f => f.Id).ToListAsync();

        var scenarios = new List<Scenario>();
        for (int i = 0; i < count; i++)
        {
            var userIndex = new Random().Next(0, userIds.Count);
            var tags = tagIds.OrderBy(x => _faker.Random.Int()).Take(5).ToList();
            var scenario = Generate(userIds[userIndex], tags, fieldIds);
            scenarios.Add(scenario);
        }

        dbContext.Scenarios.AddRange(scenarios);

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} scenarios.", count);
    }

    private Scenario Generate(UserId userId, List<TagId> tagIds, List<FieldId> fieldIds)
    {
        var title = _faker.Lorem.Word();

        var date = _faker.Date.Past(10);
        var scenario = Scenario.Create(title, userId, date);
        foreach (var tagId in tagIds)
        {
            scenario.AddTag(tagId);
        }

        foreach (var fieldId in fieldIds)
        {
            var value = _faker.Rant.Random.Word();
            scenario.AddScenarioField(fieldId, value);
        }

        return scenario;
    }
}