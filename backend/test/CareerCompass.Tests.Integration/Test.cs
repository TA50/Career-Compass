using System.Net.Http.Json;
using CareerCompass.Infrastructure.Configuration;
using Xunit.Abstractions;

namespace CareerCompass.Tests.Integration;

public class Test : IClassFixture<CareerCompassApiFactory>
{
    private readonly CareerCompassApiFactory _factory;
    private readonly ITestOutputHelper _output;

    public Test(CareerCompassApiFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
    }

    [Fact(Skip = "This is a sample test")]
    public async Task Test1()
    {
        // Arrange
        var client = _factory.CreateClient();


        // Act
        _output.WriteLine("Test1");
        var response = await client.GetAsync("/test");

        var settings = await response.Content.ReadFromJsonAsync<SmtpSettings>();


        // Assert
        Assert.NotNull(client);
    }
}