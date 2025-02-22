namespace CareerCompass.Api;

public static class ApiError
{
    private const string ApiErrorPrefix = "20";

    private static class Prefixes
    {
        public static readonly string Users = $"{ApiErrorPrefix}.{10}";
    }

    public static string FailedToGenerateEmailConfirmationLink => $"{Prefixes.Users}.{10}";
    public static string FailedToGenerateChangeForgotPasswordLink => $"{Prefixes.Users}.{20}";
    public static string FailedToSendEmailConfirmationEmail => $"{Prefixes.Users}.{30}";
    public static string RegistrationSenderConfigNotFound => $"{Prefixes.Users}.{40}";
}