using System.Reflection;
using CareerCompass.Application.Common;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CareerCompass.Application;

public static class DependencyInjections
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies([
            Assembly.GetAssembly(typeof(ApplicationAssemblyMarker)) ?? throw new InvalidOperationException()
        ]));

        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>();

        services.AddTransient(typeof(IPipelineBehavior<,>), 
            typeof(ValidationPipelineBehavior<,>));
    }
}