using CareerCompass.Core.Common.Abstractions;
using Microsoft.Extensions.Logging;

namespace CareerCompass.Tooling;

public class ToolingLoggerAdapter<T>(ILoggerFactory loggerFactory) : ILoggerAdapter<T> where T : class
{
    private readonly ILogger<T> _logger = loggerFactory.CreateLogger<T>();

    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogError(string message, params object[] args)
    {
        _logger.LogError(message, args);
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        _logger.LogError(exception, message, args);
    }
}