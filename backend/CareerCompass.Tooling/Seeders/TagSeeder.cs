using Bogus;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using CareerCompass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Tooling.Seeders;

class TagSeeder(AppDbContext dbContext, ILoggerAdapter<TagSeeder> logger) : IDatabaseSeeder
{
    public async Task SeedAsync(int count = 1)
    {
        var userIds = await dbContext.Users.Select(u => u.Id).ToListAsync();
        var tags = new List<Tag>();
        for (int i = 0; i < count; i++)
        {
            var randomIndex = new Random().Next(0, userIds.Count);
            var userId = userIds[randomIndex];
            var tag = Generate(userId);
            tags.Add(tag);
        }

        dbContext.Tags.AddRange(tags);

        await dbContext.SaveChangesAsync();
    }

    private Tag Generate(UserId userId)
    {
        var faker = new Faker();
        var name = faker.Lorem.Word();
        var tag = Tag.Create(userId, name);
        return tag;
    }
}