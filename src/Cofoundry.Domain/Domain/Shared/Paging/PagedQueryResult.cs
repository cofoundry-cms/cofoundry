namespace Cofoundry.Domain;

/// <summary>
/// <para>
/// Represents the result of a paged query, including the original 
/// query paging settings and stats about the results returned including
/// the total page count and the total number of items. 
/// </para>
/// <para>
/// If the result of the query needs to be mapped to another model type 
/// you can use the ChangeType(newItems) method to convert the result.
/// </para>
/// </summary>
public class PagedQueryResult<TResult> : IPagedQueryResult<TResult>
{
    /// <summary>
    /// The items returned from the query.
    /// </summary>
    public IReadOnlyCollection<TResult> Items { get; set; } = Array.Empty<TResult>();

    /// <summary>
    /// Total number of items in the result before paging was applied.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int PageCount { get; set; }

    /// <summary>
    /// Current (1-based) page number being returned.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items requested in the page (may not be equal to
    /// the actual number of items returned).
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Change the type of the paged result to a different type. Useful if you have to re-map a result.
    /// </summary>
    /// <typeparam name="TResultTo">Type to change the result to.</typeparam>
    /// <param name="newItems">The ordered items to add to the new result in place of the old.</param>
    /// <returns>New instance of PagedQueryResult.</returns>
    public PagedQueryResult<TResultTo> ChangeType<TResultTo>(IEnumerable<TResultTo>? newItems)
    {
        return ChangeType(newItems?.ToArray());
    }

    /// <summary>
    /// Change the type of the paged result to a different type. Useful if you have to re-map a result.
    /// </summary>
    /// <typeparam name="TResultTo">Type to change the result to.</typeparam>
    /// <param name="newItems">The ordered items to add to the new result in place of the old.</param>
    /// <returns>New instance of PagedQueryResult.</returns>
    public PagedQueryResult<TResultTo> ChangeType<TResultTo>(IReadOnlyCollection<TResultTo>? newItems)
    {
        var newResult = new PagedQueryResult<TResultTo>
        {
            Items = newItems ?? Array.Empty<TResultTo>(),
            PageCount = PageCount,
            PageNumber = PageNumber,
            PageSize = PageSize,
            TotalItems = TotalItems
        };

        return newResult;
    }

    /// <summary>
    /// Returns an empty result that represents the specified query, i.e.
    /// with the correct page number and page size, but no results.
    /// </summary>
    /// <param name="query">Query to base the result on.</param>
    /// <returns>New, empty instance of PagedQueryResult.</returns>
    public static PagedQueryResult<TResult> Empty(IPageableQuery query)
    {
        var result = new PagedQueryResult<TResult>
        {
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        return result;
    }
}