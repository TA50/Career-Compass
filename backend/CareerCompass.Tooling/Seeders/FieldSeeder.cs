using Bogus;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Users;
using CareerCompass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Tooling.Seeders;

class FieldSeeder(AppDbContext dbContext, ILoggerAdapter<FieldSeeder> logger) : IDatabaseSeeder
{
    public async Task SeedAsync(int count = 1)
    {
        logger.LogInformation("Seeding {Count} fields...", count);
        var userIds = await dbContext.Users.Select(u => u.Id).ToListAsync();
        var groups = GenerateGroups(5).ToArray();
        var fields = new List<Field>();
        for (int i = 0; i < count; i++)
        {
            var randomIndex = new Random().Next(0, userIds.Count);
            var groupIndex = new Random().Next(0, groups.Length);
            var group = groups[groupIndex];
            var userId = userIds[randomIndex];
            var field = Generate(userId, group);
            fields.Add(field);
        }

        dbContext.Fields.AddRange(fields);

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} fields.", count);
    }

    private Field Generate(UserId userId, string group)
    {
        var faker = new Faker();
        var name = faker.Lorem.Word();
        var field = Field.Create(userId, name, group);
        return field;
    }

    private IEnumerable<string> GenerateGroups(int count = 1)
    {
        var faker = new Faker();
        for (int i = 0; i < count; i++)
        {
            yield return faker.Lorem.Word();
        }
    }
}