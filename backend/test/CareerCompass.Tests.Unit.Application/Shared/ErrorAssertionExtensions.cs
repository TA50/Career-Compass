using ErrorOr;
using Shouldly;

namespace CareerCompass.Tests.Unit.Application.Shared;

public static class ErrorExtensions
{
    public static string ConvertToString(this Error src)
    {
        var metadata = "";
        if (src.Metadata is not null)
        {
            foreach (var kv in src.Metadata)
            {
                metadata += $"{kv.Key}: {kv.Value}, \n";
            }
        }

        return
            $"Type: {src.Type}, Description: {src.Description}, Code: {src.Code}, NumericType: {src.NumericType}, Metadata: {metadata}";
    }

    public static void ShouldNotBeEqual(this Error src, Error dst)
    {
        if (src.IsEqual(dst))
        {
            throw new ShouldAssertException($"Expected error {src} to not be equal to {dst}");
        }
    }

    public static void ShouldBe(this Error src, Error dst)
    {
        src.Type.ShouldBe(dst.Type);
        src.Description.ShouldBe(dst.Description);
        src.Code.ShouldBe(dst.Code);
        src.NumericType.ShouldBe(dst.NumericType);


        if (src.Metadata is not null && dst.Metadata is not null)
        {
            foreach (var kv in src.Metadata)
            {
                dst.Metadata.ShouldContainKey(kv.Key);
                dst.Metadata[kv.Key].ShouldBe(kv.Value);
            }
        }
        else
        {
            src.Metadata.ShouldBeNull();
            dst.Metadata.ShouldBeNull();
        }
    }

    public static bool IsEqual(this Error expected, Error actual)
    {
        if (expected.Type != actual.Type
            || expected.Description != actual.Description
            || expected.Code != actual.Code
            || expected.NumericType != actual.NumericType)

        {
            return false;
        }

        if (expected.Metadata is null && actual.Metadata is null)
        {
            return true;
        }

        if (expected.Metadata is not null && actual.Metadata is not null)
        {
            foreach (var kv in actual.Metadata)
            {
                if (!actual.Metadata.TryGetValue(kv.Key, out var value))
                {
                    return false;
                }

                if (value != kv.Value)
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    public static void IsNotEqual(this Error src, Error dst)
    {
        if (src.IsEqual(dst))
        {
            throw new ShouldAssertException($"Expected error {src} to not be equal to {dst}");
        }
    }

    public static void ShouldContainError(this IList<Error> actual, Error expected)
    {
        foreach (var error in actual)
        {
            if (error.IsEqual(expected))
            {
                return;
            }
        }

        var str = "";
        foreach (var e in actual)
        {
            str += e.ConvertToString() + "\n";
        }

        throw new ShouldAssertException($"Error {expected.ConvertToString()} was not found in {str}");
    }

    public static void ShouldNotContainError(this IList<Error> actual, Error expected)
    {
        foreach (var error in actual)
        {
            if (error.IsEqual(expected))
            {
                var str = "";
                foreach (var e in actual)
                {
                    str += e.ToString() + "\n";
                }

                throw new ShouldAssertException($"Error {expected} found in {str}");
            }
        }
    }
}