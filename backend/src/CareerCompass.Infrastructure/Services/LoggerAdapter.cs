using CareerCompass.Core.Common.Abstractions;
using Microsoft.Extensions.Logging;

namespace CareerCompass.Infrastructure.Services;

public class LoggerAdapter<TContext>(ILogger<TContext> logger) : ILoggerAdapter<TContext>
    where TContext : class
{
    public void LogInformation(string message, params object[] args)
    {
        logger.LogInformation(message, args);
    }

    public void LogError(string message, params object[] args)
    {
        logger.LogError(message, args);
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        logger.LogError(exception, message, args);
    }
}