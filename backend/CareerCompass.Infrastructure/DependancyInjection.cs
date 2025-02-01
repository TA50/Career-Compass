using CareerCompass.Application.Fields;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using CareerCompass.Infrastructure.Persistence;
using CareerCompass.Infrastructure.Persistence.Fields;
using CareerCompass.Infrastructure.Persistence.Scenarios;
using CareerCompass.Infrastructure.Persistence.Tags;
using CareerCompass.Infrastructure.Persistence.Users;
using CareerCompass.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CareerCompass.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opts =>
        {
            opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddIdentityCore<IdentityUser>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddIdentityApiEndpoints<IdentityUser>();
        
        services.AddTransient<IEmailSender<IdentityUser>, ConsoleEmailSender>();

        services.Configure<IdentityOptions>(opts => { opts.User.RequireUniqueEmail = true; });


        services.AddTransient<IFieldRepository, FieldRepository>();
        services.AddTransient<ITagRepository, TagRepository>();
        services.AddScoped<IScenarioRepository, ScenarioRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}