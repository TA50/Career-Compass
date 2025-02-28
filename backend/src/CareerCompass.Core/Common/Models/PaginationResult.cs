using System.Text.Json.Serialization;

namespace CareerCompass.Core.Common.Models;

public record PaginationResult<T>
{
    public PaginationResult(IList<T> pageItems, int totalItems, int pageSize, int page)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize, nameof(pageSize));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(page, nameof(page));
        ArgumentOutOfRangeException.ThrowIfNegative(totalItems, nameof(totalItems));

        Items = pageItems.AsReadOnly();

        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        NextPage = page < TotalPages ? page + 1 : null;
        PreviousPage = page > 1 ? page - 1 : null;
        Page = page;
    }

    [JsonInclude] public IReadOnlyList<T> Items { get; private set; } = new List<T>();
    [JsonInclude] public int TotalPages { get; private set; }
    [JsonInclude] public int? NextPage { get; private set; }
    [JsonInclude] public int Page { get; private set; }
    [JsonInclude] public int? PreviousPage { get; private set; }

    public static PaginationResult<T> Empty => new(Enumerable.Empty<T>().ToList(), 0, null, 0, null);

    public static PaginationResult<TDestination> Map<TSource, TDestination>(PaginationResult<TSource> source,
        Func<TSource, TDestination> selector)
    {
        return new PaginationResult<TDestination>(
            items: source.Items.Select(selector).ToList(),
            totalPages: source.TotalPages,
            nextPage: source.NextPage,
            page: source.Page,
            previousPage: source.PreviousPage
        );
    }


    [JsonConstructor]
    private PaginationResult()
    {
        
    }


    private PaginationResult(
        IList<T> items,
        int totalPages,
        int? nextPage,
        int page,
        int? previousPage
    )
    {
        Items = items.AsReadOnly();
        TotalPages = totalPages;
        NextPage = nextPage;
        Page = page;
        PreviousPage = previousPage;
    }
}