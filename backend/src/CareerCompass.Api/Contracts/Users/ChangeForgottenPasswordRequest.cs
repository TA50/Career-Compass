namespace CareerCompass.Api.Contracts.Users;

public record ChangeForgottenPasswordRequest(
    string Email,
    string Code,
    string NewPassword,
    string ConfirmNewPassword
);