using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Tests.Integration;

public static class AssertionExtensions
{
    public static void ShouldBeEquivalentTo(this ValidationProblemDetails? actual, ValidationProblemDetails expected)
    {
        actual.Should().NotBeNull();
        actual.Title.Should().Be(expected.Title);
        actual.Status.Should().Be(expected.Status);


        foreach (var (key, errors) in expected.Errors)
        {
            foreach (var error in errors)
            {
                actual.Errors.Should().ContainKey(key).WhoseValue.Should().Contain(error);
            }
        }
    }

    public static void ShouldBeEquivalentTo(this ProblemDetails? actual, ProblemDetails expected)
    {
        actual.Should().NotBeNull();

        actual.Title.Should().Be(expected.Title);
        actual.Status.Should().Be(expected.Status);
        actual.Detail.Should().Be(expected.Detail);


        foreach (var expectedExtension in expected.Extensions)
        {
            var actualExtension = actual.Extensions[expectedExtension.Key]?.ToString() ?? string.Empty;
            actualExtension.Should().Be(expectedExtension.Value?.ToString() ?? string.Empty);
        }
    }
}