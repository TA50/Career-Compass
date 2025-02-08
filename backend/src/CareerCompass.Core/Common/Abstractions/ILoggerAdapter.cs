namespace CareerCompass.Core.Common.Abstractions;

public interface ILoggerAdapter<out TContext> where TContext : class
{
    public void LogInformation(string message, params object[] args);

    // public void LogWarning(string message, params object[] args);

    public void LogError(string message, params object[] args);
    public void LogError(Exception exception, string message, params object[] args);

    // public void LogCritical(string message, params object[] args);
    // public void LogCritical(Exception exception, string message, params object[] args);
    //
    // public void LogDebug(string message, params object[] args);
    // public void LogDebug(Exception exception, string message, params object[] args);
}