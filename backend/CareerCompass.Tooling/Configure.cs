using CareerCompass.Api;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Infrastructure.Persistence;
using CareerCompass.Infrastructure.Services;
using CareerCompass.Tooling.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CareerCompass.Tooling.Importers;

namespace CareerCompass.Tooling;

partial class App
{
    public App()
    {
        var baseDir = Directory.GetCurrentDirectory();
        var confiFilePath = baseDir + "/../../../appsettings.Development.json";
        var services = new ServiceCollection();
        var configurationBuilder = new ConfigurationBuilder()
            .AddUserSecrets<ApiMarker>()
            .AddJsonFile(confiFilePath)
            .AddEnvironmentVariables();
        Configuration = configurationBuilder.Build();
        services.AddSingleton(Configuration);
        Configure(services, Configuration);

        ServiceProvider = services.BuildServiceProvider();
    }

    private IServiceProvider ServiceProvider { init; get; }
    private IConfiguration Configuration { init; get; }


    private void Configure(IServiceCollection services, IConfiguration configuration)
    {
        var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        services.AddSingleton(loggerFactory);
        var dbConnectionString = configuration.GetConnectionString("FalconNgrokDockerForwarded");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseLoggerFactory(LoggerFactory.Create(builder => { }));
            options.UseSqlServer(dbConnectionString);
            options.LogTo(Console.WriteLine, LogLevel.None);
            options.EnableSensitiveDataLogging(false);
        }, ServiceLifetime.Transient);
        services.AddTransient<UserSeeder>();
        services.AddTransient<TagSeeder>();
        services.AddTransient<FieldSeeder>();
        services.AddTransient<ScenarioSeeder>();
        services.AddTransient<ICryptoService, CryptoService>();
        services.AddTransient<CsvImporter>();
        services.AddTransient(typeof(ILoggerAdapter<>), typeof(ToolingLoggerAdapter<>));
    }
}