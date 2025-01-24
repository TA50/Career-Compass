using Microsoft.AspNetCore.Identity;


namespace CareerCompass.Infrastructure.Services;

public class ConsoleEmailSender : IEmailSender<IdentityUser>
{
    public Task SendConfirmationLinkAsync(IdentityUser user, string email, string confirmationLink)
    {
        Console.WriteLine(@$"
    Dear {user.UserName}
    Confirm your email by click on : 
    {confirmationLink}
");
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(IdentityUser user, string email, string resetLink)
    {
        Console.WriteLine(@$"
    Dear {user.UserName}
    reset your password by visiting: 
    {resetLink}
");
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(IdentityUser user, string email, string resetCode)
    {
        Console.WriteLine(@$"
    Dear {user.UserName}
    this is your password reset code: 
    {resetCode}
");
        return Task.CompletedTask;
    }
}