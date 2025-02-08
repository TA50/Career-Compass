using CareerCompass.Core.Users;
using Microsoft.AspNetCore.Identity;

namespace CareerCompass.Infrastructure.Persistence;

public class ApplicationIdentityUser : IdentityUser<UserId>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public User User { get; set; }
}