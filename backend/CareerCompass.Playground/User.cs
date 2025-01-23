using Microsoft.AspNetCore.Identity;

namespace CareerCompass.Playground;

public class User : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Bio { get; set; }
}