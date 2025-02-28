using CareerCompass.Core.Common.Abstractions;
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
        var csvImporter = ServiceProvider.GetRequiredService<CsvImporter>();
        var logger = ServiceProvider.GetRequiredService<ILoggerAdapter<App>>();
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