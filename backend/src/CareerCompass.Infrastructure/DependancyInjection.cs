using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Email;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Infrastructure.Configuration;
using CareerCompass.Infrastructure.Persistence;
using CareerCompass.Infrastructure.Persistence.Repositories;
using CareerCompass.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CareerCompass.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionBuilder)
    {
        services.AddDbContext<AppDbContext>(optionBuilder);


        services.AddTransient<IFieldRepository, FieldRepository>();
        services.AddTransient<ITagRepository, TagRepository>();
        services.AddTransient<IScenarioRepository, ScenarioRepository>();
        services.AddTransient<IUserRepository, UserRepository>();


        services.AddTransient(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

        services.AddTransient<IEmailSender, SmtpEmailService>();
        services.AddTransient<ICryptoService, CryptoService>();
    }
}