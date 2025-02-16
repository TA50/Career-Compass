using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Users.Commands.GenerateForgotPasswordCode;
using CareerCompass.Tests.Fakers.Core;
using NSubstitute;

namespace CareerCompass.Tests.Unit.Core.Users;

public class GenerateForgotPasswordCodeTests
{
    private readonly UserFaker _userFaker = new();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    
    private readonly ILoggerAdapter<GenerateForgotPasswordCodeCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<GenerateForgotPasswordCodeCommandHandler>>();

    private readonly GenerateForgotPasswordCodeCommandHandler _sut;

    public GenerateForgotPasswordCodeTests()
    {
        _sut = new(_userRepository, _logger);
    }

    [Fact]
    public async Task ShouldGenerateForgotPasswordCode_WhenInputIsValid()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public async Task ShouldReturnUserNotFound_WhenEmailDoesNotBelongToUser()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public async Task ShouldReturnForgotPasswordFailed_WhenDbFails()
    {
        // Arrange

        // Act

        // Assert
    }
}