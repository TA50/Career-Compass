using CareerCompass.Core.Tags;
using CareerCompass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
// optionsBuilder.LogTo(Console.WriteLine);

await using var dbContext = new AppDbContext(optionsBuilder.Options);

TagId[] tagIds = [TagId.Create("511ca08e-b0fc-41c6-8e00-e1d4673a1c86")];
var tagId = tagIds[0];

var scenario = await dbContext.Scenarios
    .Where(t => t.TagIds.Any())
    .Where(scenario => scenario.TagIds.Any(x => 
        tagIds.Select(x => x).Contains(x)))
    .FirstOrDefaultAsync();

Console.WriteLine(scenario.Id);

// ConfigureScenarioTagIds