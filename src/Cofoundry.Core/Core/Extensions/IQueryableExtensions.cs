namespace Cofoundry.Core;

public static class IQueryableExtensions
{
    /// <summary>
    /// Removes nullable entries from the sequence.
    /// </summary>
    /// <param name="source">
    /// Queryable instance to filter.
    /// </param>
    public static IQueryable<T> WhereNotNull<T>(this IQueryable<T?> source)
        where T : struct
    {
        return source
            .Where(s => s != null)
            .Cast<T>();
    }

    /// <summary>
    /// Removes nullable entries from the sequence.
    /// </summary>
    /// <param name="source">
    /// Queryable instance to filter.
    /// </param>
    public static IQueryable<T> WhereNotNull<T>(this IQueryable<T?> source)
    {
        return source
            .Where(s => s != null)
            .Cast<T>();
    }

    /// <summary>
    /// Removes nullable null or empty strings from the sequence.
    /// </summary>
    /// <param name="source">
    /// Queryable instance to filter.
    /// </param>
    public static IQueryable<string> WhereNotNullOrEmpty(this IQueryable<string?> source)
    {
        return source
            .Where(s => !string.IsNullOrEmpty(s))
            .Cast<string>();
    }

    /// <summary>
    /// Removes nullable null, empty or strings that contain only whitespace 
    /// from the sequence.
    /// </summary>
    /// <param name="source">
    /// Queryable instance to filter.
    /// </param>
    public static IQueryable<string> WhereNotNullOrWhitespace(this IQueryable<string?> source)
    {
        return source
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Cast<string>();
    }
}
