using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Infrastructure.Persistence;
using CareerCompass.Infrastructure.Persistence.Repositories;
using CareerCompass.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CareerCompass.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionBuilder)
    {
        services.AddDbContext<AppDbContext>(optionBuilder);
        services.AddIdentityCore<ApplicationIdentityUser>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddIdentityApiEndpoints<ApplicationIdentityUser>();

        services.AddTransient<IEmailSender<IdentityUser>, ConsoleEmailSender>();
        services.Configure<IdentityOptions>(opts => { opts.User.RequireUniqueEmail = true; });

        services.AddTransient<IFieldRepository, FieldRepository>();
        services.AddTransient<ITagRepository, TagRepository>();
        services.AddTransient<IScenarioRepository, ScenarioRepository>();
        services.AddTransient<IUserRepository, UserRepository>();


        services.AddTransient(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
    }
}