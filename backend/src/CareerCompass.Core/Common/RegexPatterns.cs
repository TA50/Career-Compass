namespace CareerCompass.Core.Common;

public static class RegexPatterns
{
    public const string PasswordSpecialCharacters = @"@$!%*?&\-_";

    public const string Password =
        @$"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[{PasswordSpecialCharacters}])[A-Za-z\d[{PasswordSpecialCharacters}]{"{8,}"}$";
}