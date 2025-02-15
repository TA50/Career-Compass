using CareerCompass.Api;
using CareerCompass.Infrastructure.Configuration;
using CareerCompass.Infrastructure.Persistence;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace CareerCompass.Tests.Integration;

public class CareerCompassApiFactory : WebApplicationFactory<ApiMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithEnvironment("MSSQL_SA_PASSWORD", "yourStrong(!)Password")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithPortBinding(5433, 1433)
        .Build();

    private const int MailhogWebUiPort = 3025;
    private const int MailhogSmtpPort = 6066;

    private readonly IContainer _mailhogContainer = new ContainerBuilder()
        .WithImage("mailhog/mailhog")
        .WithPortBinding(MailhogWebUiPort, 8025)
        .WithPortBinding(MailhogSmtpPort, 1025)
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var dbContextSd = services.First(descriptor => descriptor.ServiceType == typeof(AppDbContext));
            services.Remove(dbContextSd);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_dbContainer.GetConnectionString());
            });

            services.Configure<SmtpSettings>(a =>
            {
                a.Host = "localhost";
                a.Port = MailhogSmtpPort;
                a.EnableSsl = false;
                a.UserName = string.Empty;
                a.Password = string.Empty;
            });
        });

        base.ConfigureWebHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _mailhogContainer.StartAsync();

        // Apply migrations
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await _mailhogContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}