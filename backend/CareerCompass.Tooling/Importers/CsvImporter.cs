using System.Globalization;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using CareerCompass.Infrastructure.Persistence;
using CsvHelper;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Tooling.Importers;

class CsvImporter(AppDbContext dbContext, ILoggerAdapter<CsvImporter> logger, ICryptoService cryptoService)
{
    public async Task Import(string filePath, UserId? userId = null)
    {
        logger.LogInformation("Importing scenarios from csv file");
        userId ??= await CreateUser();
        var records = Read(filePath);

        logger.LogInformation("Found {Count} records in the file", records.Count);
        var tags = await CreateTags(records, userId);
        logger.LogInformation("Added {count} tags", tags.Count);
        var situation = Field.Create(userId, "Situation", "STAR");
        var task = Field.Create(userId, "Task", "STAR");
        var action = Field.Create(userId, "Action", "STAR");
        var result = Field.Create(userId, "Result", "STAR");
        dbContext.Fields.AddRange(situation, task, action, result);
        await dbContext.SaveChangesAsync();

        foreach (var record in records)
        {
            var scenario = CreateScenario(record: record,
                allTags: tags,
                userId: userId,
                situation: situation,
                task: task,
                action: action,
                result: result);

            dbContext.Scenarios.Add(scenario);
        }

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Added {count} scenarios", records.Count);
    }

    private Scenario CreateScenario(CsvRecord record, List<Tag> allTags,
        UserId userId, Field situation, Field task, Field action, Field result)
    {
        var scenario = Scenario.Create(record.Title, userId, DateTime.Parse(record.Date));
        var providedTags = record.Tags.Split(",").Select(x => x.Trim()).ToList();
        var tags = allTags.Where(t => providedTags.Contains(t.Name)).ToList();
        foreach (var tag in tags)
        {
            scenario.AddTag(tag.Id);
        }

        scenario.AddScenarioField(situation.Id, record.Situation);
        scenario.AddScenarioField(task.Id, record.Task);
        scenario.AddScenarioField(action.Id, record.Action);
        scenario.AddScenarioField(result.Id, record.Result);

        return scenario;
    }

    private async Task<UserId> CreateUser()
    {
        var password = "Password123!";
        var hash = cryptoService.Hash(password);
        var user = User.Create(
            "awadosman@gmail.com",
            hash,
            "Awad",
            "Osman"
        );
        var code = user.GenerateEmailConfirmationCode(TimeSpan.FromHours(1));
        user.ConfirmEmail(code);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return user.Id;
    }

    private async Task<List<Tag>> CreateTags(List<CsvRecord> records, UserId userId)
    {
        var uniqueTags = records
            .SelectMany(r => r.Tags.Split(','))
            .Select(x => x.Trim())
            .Distinct();


        foreach (var tagName in uniqueTags)
        {
            var tag = Tag.Create(userId, tagName);
            dbContext.Tags.Add(tag);
        }

        await dbContext.SaveChangesAsync();

        return await dbContext.Tags.ToListAsync();
    }


    private List<CsvRecord> Read(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<CsvRecord>().ToList();
    }
}