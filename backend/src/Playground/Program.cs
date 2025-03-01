using CareerCompass.Api;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using CareerCompass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();
builder.Configuration.AddUserSecrets<ApiMarker>();
builder.Services.AddDbContext<AppDbContext>(
    opts => { opts.UseSqlServer(builder.Configuration.GetConnectionString("TestConnection")); },
    ServiceLifetime.Transient);

var host = builder.Build();

var dbContext = host.Services.GetRequiredService<AppDbContext>();

await dbContext.Database.EnsureDeletedAsync();
await dbContext.Database.EnsureCreatedAsync();

var user = User.Create(
    "test@gmail.com", "pas", "fir", "last");
var tag = Tag.Create(user.Id, "test");

dbContext.Users.Add(user);
dbContext.Tags.Add(tag);

var scenario = Scenario.Create("scenario 1", user.Id, DateTime.Now);
scenario.AddTag(tag.Id);

await dbContext.Scenarios.AddAsync(scenario);

await dbContext.SaveChangesAsync();

var s = await dbContext.Scenarios.AsNoTracking().ToListAsync();

foreach (var sc in s)
{
    Console.WriteLine(sc.Title + "  :  " + sc.TagIds[0]);
}

var scenario2 = Scenario.Create("scenario 2", user.Id, DateTime.Now);
var sTagId = TagId.Create(tag.Id.Value);
scenario2.AddTag(sTagId);
await dbContext.Scenarios.AddAsync(scenario2);


await dbContext.SaveChangesAsync();
s = await dbContext.Scenarios.AsNoTracking().ToListAsync();
foreach (var sc in s)

{
    Console.WriteLine(sc.TagIds[0]);
}