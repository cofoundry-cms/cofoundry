namespace Cofoundry.Domain.Data;

public static class PageDirectoryQueryExtensions
{
    /// <summary>
    /// Fitlers the collection to only include pages with the 
    /// specified <paramref name="pageDirectoryId"/> primary key.
    /// </summary>
    /// <param name="pageDirectories">
    /// Queryable instance to filter.
    /// </param>
    /// <param name="pageDirectoryId">PageDirectoryId to filter by.</param>
    public static IQueryable<PageDirectory> FilterById(this IQueryable<PageDirectory> pageDirectories, int pageDirectoryId)
    {
        var result = pageDirectories.Where(i => i.PageDirectoryId == pageDirectoryId);

        return result;
    }

    /// <summary>
    /// Fitlers the collection to only include pages with the 
    /// specified <paramref name="pageDirectoryId"/> primary key.
    /// </summary>
    /// <param name="pageDirectories">
    /// Enumerable to filter.
    /// </param>
    /// <param name="pageDirectoryId">PageDirectoryId to filter by.</param>
    public static IEnumerable<PageDirectory> FilterById(this IEnumerable<PageDirectory> pageDirectories, int pageDirectoryId)
    {
        var result = pageDirectories.Where(i => i.PageDirectoryId == pageDirectoryId);

        return result;
    }
}
