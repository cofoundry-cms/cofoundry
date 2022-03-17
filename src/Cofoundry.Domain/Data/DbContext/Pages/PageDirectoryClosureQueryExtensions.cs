namespace Cofoundry.Domain.Data;

public static class PageDirectoryClosureQueryExtensions
{
    /// <summary>
    /// Fitlers the collection based on the on the DescendantPageDirectoryId 
    /// field i.e. this will only return the self-referencing record and
    /// ancestors of the specified directory.
    /// </summary>
    /// <param name="pageDirectoryId">PageDirectoryId to filter by on the DescendantPageDirectoryId field.</param>
    public static IQueryable<PageDirectoryClosure> FilterByDescendantId(this IQueryable<PageDirectoryClosure> closures, int pageDirectoryId)
    {
        var result = closures.Where(i => i.DescendantPageDirectoryId == pageDirectoryId);

        return result;
    }

    /// <summary>
    /// Fitlers the collection based on the on the DescendantPageDirectoryId 
    /// field i.e. this will only return the self-referencing record and
    /// ancestors of the specified directory.
    /// </summary>
    /// <param name="pageDirectoryId">PageDirectoryId to filter by on the DescendantPageDirectoryId field.</param>
    public static IEnumerable<PageDirectoryClosure> FilterByDescendantId(this IEnumerable<PageDirectoryClosure> closures, int pageDirectoryId)
    {
        var result = closures.Where(i => i.DescendantPageDirectoryId == pageDirectoryId);

        return result;
    }

    /// <summary>
    /// Fitlers the collection based on the on the AncestorPageDirectoryId 
    /// field i.e. this will only return the self-referencing record and
    /// decendants of the specified directory.
    /// </summary>
    /// <param name="pageDirectoryId">PageDirectoryId to filter by on the AncestorPageDirectoryId field.</param>
    public static IQueryable<PageDirectoryClosure> FilterByAncestorId(this IQueryable<PageDirectoryClosure> closures, int pageDirectoryId)
    {
        var result = closures.Where(i => i.AncestorPageDirectoryId == pageDirectoryId);

        return result;
    }

    /// <summary>
    /// Fitlers the collection based on the on the AncestorPageDirectoryId 
    /// field i.e. this will only return the self-referencing record and
    /// decendants of the specified directory.
    /// </summary>
    /// <param name="pageDirectoryId">PageDirectoryId to filter by on the AncestorPageDirectoryId field.</param>
    public static IEnumerable<PageDirectoryClosure> FilterByAncestorId(this IEnumerable<PageDirectoryClosure> closures, int pageDirectoryId)
    {
        var result = closures.Where(i => i.AncestorPageDirectoryId == pageDirectoryId);

        return result;
    }

    /// <summary>
    /// Filters to include only the self-referencing records, where the ancestor and descenent are the same.
    /// </summary>
    public static IQueryable<PageDirectoryClosure> FilterSelfReferencing(this IQueryable<PageDirectoryClosure> closures)
    {
        var result = closures.Where(i => i.AncestorPageDirectoryId == i.DescendantPageDirectoryId);

        return result;
    }

    /// <summary>
    /// Filters to include only the self-referencing records, where the ancestor and descenent are the same.
    /// </summary>
    public static IEnumerable<PageDirectoryClosure> FilterSelfReferencing(this IEnumerable<PageDirectoryClosure> closures)
    {
        var result = closures.Where(i => i.AncestorPageDirectoryId == i.DescendantPageDirectoryId);

        return result;
    }

    /// <summary>
    /// Removes the self-referencing records, where the ancestor and descendant are the same.
    /// </summary>
    public static IQueryable<PageDirectoryClosure> FilterNotSelfReferencing(this IQueryable<PageDirectoryClosure> closures)
    {
        var result = closures.Where(i => i.AncestorPageDirectoryId != i.DescendantPageDirectoryId);

        return result;
    }

    /// <summary>
    /// Removes the self-referencing records, where the ancestor and descendant are the same.
    /// </summary>
    public static IEnumerable<PageDirectoryClosure> FilterNotSelfReferencing(this IEnumerable<PageDirectoryClosure> closures)
    {
        var result = closures.Where(i => i.AncestorPageDirectoryId != i.DescendantPageDirectoryId);

        return result;
    }
}
