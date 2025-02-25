using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Events;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Scenarios.EventHandlers;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using CareerCompass.Infrastructure.Persistence;
using CareerCompass.Infrastructure.Persistence.Repositories;
using CareerCompass.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


var builder = Host.CreateApplicationBuilder();
builder.Configuration.AddUserSecrets<Program>();
builder.Logging.ClearProviders();
var connection = builder.Configuration.GetConnectionString("TestConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connection);
    options.LogTo(_ => { });
}, ServiceLifetime.Scoped);

builder.Services.AddTransient<IScenarioRepository, ScenarioRepository>();
builder.Services.AddTransient<IFieldRepository, FieldRepository>();
builder.Services.AddTransient<ITagRepository, TagRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

var host = builder.Build();

var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;
using var dbContext = services.GetRequiredService<AppDbContext>();

dbContext.Database.EnsureDeleted();
dbContext.Database.EnsureCreated();

var scenarioRepository = services.GetRequiredService<IScenarioRepository>();
var tagRepository = services.GetRequiredService<ITagRepository>();
var userRepository = services.GetRequiredService<IUserRepository>();
var logger = services.GetRequiredService<ILoggerAdapter<FieldDeletedScenarioHandler>>();

var user = User.Create("email", "password", "test", "user");
var tag = Tag.Create(user.Id, "tag");
var tag2 = Tag.Create(user.Id, "tag2");
var scenario = Scenario.Create("title", user.Id, DateTime.Now);
scenario.AddTag(tag.Id);
scenario.AddTag(tag2.Id);
await userRepository.Create(user);
await scenarioRepository.Create(scenario);
await tagRepository.Create(tag);
await tagRepository.Create(tag2);


PrintValues("Init values");

var uscenario = await scenarioRepository.Get(scenario.Id, true);
uscenario.RemoveTag(tag.Id);

await scenarioRepository.Save();

PrintValues("Updated values");

return;


void PrintValues(string message)
{
    Console.WriteLine(message);
    var fetchedScenario = dbContext.Scenarios
        .AsNoTracking()
        .First();
    foreach (var sf in fetchedScenario.TagIds)
    {
        Console.WriteLine($"{sf}");
    }
}