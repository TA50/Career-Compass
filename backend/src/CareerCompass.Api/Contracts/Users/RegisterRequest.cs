using CareerCompass.Core.Users.Commands.Register;

namespace CareerCompass.Api.Contracts.Users;

public class RegisterRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    public RegisterCommand ToRegisterCommand()
    {
        return new RegisterCommand(
            FirstName,
            LastName,
            Email,
            Password,
            ConfirmPassword
        );
    }
}