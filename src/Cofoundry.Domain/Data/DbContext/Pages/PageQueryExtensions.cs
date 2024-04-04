namespace Cofoundry.Domain.Data;

public static class PageQueryExtensions
{
    /// <summary>
    /// Fitlers the collection to only include pages with the 
    /// specified <paramref name="pageId"/>.
    /// </summary>
    /// <param name="pages">
    /// Queryable instance to filter.
    /// </param>
    /// <param name="pageId">PageId to filter by.</param>
    public static IQueryable<Page> FilterById(this IQueryable<Page> pages, int pageId)
    {
        var result = pages
            .Where(i => i.PageId == pageId);

        return result;
    }

    /// <summary>
    /// Fitlers the collection to only include pages parented to the 
    /// specified directory.
    /// </summary>
    /// <param name="pages">
    /// Queryable instance to filter.
    /// </param>
    /// <param name="pageDirectoryId">PageDirectoryId to filter by.</param>
    public static IQueryable<Page> FilterByPageDirectoryId(this IQueryable<Page> pages, int pageDirectoryId)
    {
        var result = pages
            .Where(i => i.PageDirectoryId == pageDirectoryId);

        return result;
    }

    /// <summary>
    /// Filters the collection to only include versions that are
    /// not deleted and not in deleted directories.
    /// </summary>
    /// <param name="pages">
    /// Queryable instance to filter.
    /// </param>
    public static IQueryable<Page> FilterActive(this IQueryable<Page> pages)
    {
        // Not currently filtered, but may need to add locale filtering in here
        // in a later version so will leave this here for now.
        return pages;
    }
}
