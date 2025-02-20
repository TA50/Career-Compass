using CareerCompass.Api;
using CareerCompass.Core.Common;
using CareerCompass.Infrastructure.Configuration;
using CareerCompass.Infrastructure.Persistence;
using CareerCompass.Tests.Integration.EmailServerClient;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;

namespace CareerCompass.Tests.Integration;

public class CareerCompassApiFactory : WebApplicationFactory<ApiMarker>, IAsyncLifetime
{
    internal AppDbContext DbContext { get; private set; }
    public IEmailServerClient EmailServerClient { get; private set; }

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithEnvironment("MSSQL_SA_PASSWORD", "yourStrong(!)Password")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithPortBinding(5433, 1433)
        .Build();

    private const int MailhogWebUiPort = 3025;
    private const int MailhogSmtpPort = 6066;

    private readonly IContainer _smtpContainer = new ContainerBuilder()
        .WithImage("axllent/mailpit")
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

            services.RemoveAll(typeof(SmtpSettings));
            services.AddSingleton(new SmtpSettings()
            {
                Host = "localhost",
                Port = MailhogSmtpPort,
                EnableSsl = false,
                UserName = string.Empty,
                Password = string.Empty
            });

            services.RemoveAll(typeof(CoreSettings));
            services.AddSingleton(new CoreSettings()
            {
                EmailConfirmationCodeLifetimeInHours = 6,
                ForgotPasswordCodeLifetimeInHours = 6
            });
        });

        base.ConfigureWebHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _smtpContainer.StartAsync();

        // Apply migrations
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_dbContainer.GetConnectionString())
            .Options;
        DbContext = new AppDbContext(dbContextOptions);
        await DbContext.Database.MigrateAsync();

        EmailServerClient = new MailPitClient(MailhogWebUiPort);
    }

    public new async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _dbContainer.DisposeAsync();
        await _smtpContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}