using System.Collections.Immutable;
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
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace CareerCompass.Tooling;

partial class App
{
    public App()
    {
        var services = new ServiceCollection();
        var configurationBuilder = new ConfigurationBuilder()
            .AddUserSecrets<ApiMarker>()
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
        var dbConnectionString = configuration.GetConnectionString("TestConnection");
        services.AddDbContext<AppDbContext>(options =>
        {
            options.LogTo(_ => { });
            options.UseSqlServer(dbConnectionString);
        });
        services.AddTransient<UserSeeder>();
        services.AddTransient<TagSeeder>();
        services.AddTransient<FieldSeeder>();
        services.AddTransient<ScenarioSeeder>();
        services.AddTransient<ICryptoService, CryptoService>();
        services.AddTransient(typeof(ILoggerAdapter<>), typeof(ToolingLoggerAdapter<>));
    }
}