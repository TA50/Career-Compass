using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Users;
using CareerCompass.Infrastructure.Persistence;
using CareerCompass.Tooling.Importers;
using CareerCompass.Tooling.Seeders;
using Microsoft.Extensions.DependencyInjection;

namespace CareerCompass.Tooling;

partial class App
{
    public Task Run() => Main();

    private async Task Main()
    {
        // await ClearDatabase();
        
        var csvImporter = ServiceProvider.GetRequiredService<CsvImporter>();
        var logger = ServiceProvider.GetRequiredService<ILoggerAdapter<App>>();
        var dbContext = ServiceProvider.GetRequiredService<AppDbContext>();


        var user = User.Create("test@test.com", "password", "test", "user");
        await dbContext.Users.AddAsync(user);
        logger.LogInformation("Starting the application");
        string? filePath = Configuration["DataFile"];
        if (filePath is not null)
        {
            await csvImporter.Import(filePath);
        }
        else
        {
            logger.LogError("DataFile configuration is missing");
        }
    }

    private async Task ClearDatabase()
    {
        var dbContext = ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = ServiceProvider.GetRequiredService<ILoggerAdapter<App>>();
        logger.LogInformation("Clearing the database");
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        logger.LogInformation("Database cleared");
    }
}