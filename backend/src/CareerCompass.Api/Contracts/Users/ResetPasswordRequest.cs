namespace CareerCompass.Api.Contracts.Users;

public record ResetPasswordRequest(string OldPassword, string NewPassword, string ConfirmNewPassword);