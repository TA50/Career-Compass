using System.Reflection;
using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CareerCompass.Core;

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