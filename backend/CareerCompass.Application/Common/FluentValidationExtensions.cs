namespace CareerCompass.Application.Common;

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
}