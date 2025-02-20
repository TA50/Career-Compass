using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Api.Contracts.Users;

public record ChangeEmailRequest(string OldPassword, string Email);