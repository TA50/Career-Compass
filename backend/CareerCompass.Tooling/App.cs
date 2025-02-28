using CareerCompass.Tooling.Seeders;
using Microsoft.Extensions.DependencyInjection;

namespace CareerCompass.Tooling;

partial class App
{
    public Task Run() => Main();

    private async Task Main()
    {
        var userSeeder = ServiceProvider.GetRequiredService<UserSeeder>();
        var tagSeeder = ServiceProvider.GetRequiredService<TagSeeder>();
        var fieldSeeder = ServiceProvider.GetRequiredService<FieldSeeder>();
        var scenarioSeeder = ServiceProvider.GetRequiredService<ScenarioSeeder>();


        await userSeeder.SeedAsync(10);
        await tagSeeder.SeedAsync(10);
        await fieldSeeder.SeedAsync(4);
        await scenarioSeeder.SeedAsync(5);
    }
}