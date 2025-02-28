using CareerCompass.Core.Common.Models;

namespace CareerCompass.Tooling.Seeders;

public interface IDatabaseSeeder
{
    Task SeedAsync(int count = 1);
}