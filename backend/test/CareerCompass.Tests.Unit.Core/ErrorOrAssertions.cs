using FluentAssertions.Primitives;

namespace CareerCompass.Tests.Unit.Core;

using ErrorOr;
using FluentAssertions;

public static class ErrorOrAssertions
{
    public static void ShouldBeEquivalentTo(this Error actual, Error expected)
    {
        actual.Code.Should().Be(expected.Code);
        actual.Description.Should().Be(expected.Description);
        actual.Type.Should().Be(expected.Type);
        actual.NumericType.Should().Be(expected.NumericType);
        if (expected.Metadata == null)
        {
            actual.Metadata.Should().BeNull();
            return;
        }

        foreach (var metaData in expected.Metadata)
        {
            actual.Metadata.Should().ContainEquivalentOf(metaData);
        }
    }


    public static void ShouldContainEquivalentOf(this IEnumerable<Error> actual, Error expected)
    {
        actual.Should().Contain(x => x.EqualTo(expected));
    }

    public static bool EqualTo(this Error actual, Error expected)
    {
        var simpleChecks = actual.Code == expected.Code &&
                           actual.Description == expected.Description &&
                           actual.Type == expected.Type &&
                           actual.NumericType == expected.NumericType;

        if (!simpleChecks)
        {
            return false;
        }

        var metaDataChecks = false;
        if (expected.Metadata == null)
        {
            metaDataChecks = actual.Metadata == null;
            return metaDataChecks && simpleChecks;
        }

        if (actual.Metadata == null)
        {
            return false;
        }


        foreach (var metaData in expected.Metadata)
        {
            if (actual.Metadata.TryGetValue(metaData.Key, out var value))
            {
                metaDataChecks = value == metaData.Value;
            }
            else
            {
                metaDataChecks = false;
                break;
            }
        }

        return metaDataChecks && simpleChecks;
    }
}