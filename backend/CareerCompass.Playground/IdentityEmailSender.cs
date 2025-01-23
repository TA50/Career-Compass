using Microsoft.AspNetCore.Identity;

namespace CareerCompass.Playground;

public class IdentityEmailSender : IEmailSender<User>
{
    public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        Console.WriteLine(@$"
    Dear {user.UserName}
    Confirm your email by click on : 
    {confirmationLink}
");
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        Console.WriteLine(@$"
    Dear {user.UserName}
    reset your password by visiting: 
    {resetLink}
");
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        Console.WriteLine(@$"
    Dear {user.UserName}
    this is your password reset code: 
    {resetCode}
");
        return Task.CompletedTask;
    }
}