namespace CareerCompass.Application.Common;

public static class ErrorMetaDataKey
{
    public const string Title = "title";
}

public static class ApplicationErrorPrefix
{
    private const int _prefix = 10;

    public static string Create(ApplicationErrorType code)
    {
        return $"{_prefix}.{(int)code}";
    }

    public enum ApplicationErrorType
    {
        Scenario = 10,
        Tag = 20,
        User = 30,
        Field = 40
    }
}