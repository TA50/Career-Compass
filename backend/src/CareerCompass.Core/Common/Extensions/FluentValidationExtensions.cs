using FluentValidation;

namespace CareerCompass.Core.Common.Extensions;

public static class FluentValidationExtensions
{
    public static bool IsDistinct<TSource, TResult>(this IEnumerable<TSource> elements, Func<TSource, TResult> selector)
    {
        var hashSet = new HashSet<TResult>();
        foreach (var element in elements.Select(selector))
        {
            if (!hashSet.Contains(element))
                hashSet.Add(element);
            else
                return false;
        }

        return true;
    }

    public static IRuleBuilderOptions<T, IEnumerable<TSource>> IsDistinct<T, TSource, TResult>(
        this IRuleBuilderInitial<T, IEnumerable<TSource>> ruleBuilder, Func<TSource, TResult> selector)
    {
        return ruleBuilder.Must(x => x.IsDistinct<TSource, TResult>(selector));
    }

    public static IRuleBuilderOptions<T, string> IsGuid<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(guidString =>
        {
            var check = Guid.TryParse(guidString, out _);

            return check;
        }).WithMessage("The provided value could not be parsed as Guid");
    }
}