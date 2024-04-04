namespace Cofoundry.Domain.Data;

public static class PageAccessRuleQueryExtensions
{
    /// <summary>
    /// Filters the collection to only include access rules associated with
    /// a specific page.
    /// </summary>
    /// <param name="pageAccessRules">
    /// Queryable instance to filter.
    /// </param>
    /// <param name="pageId">Id of the <see cref="Page"/> to filter on.</param>
    public static IQueryable<PageAccessRule> FilterByPageId(this IQueryable<PageAccessRule> pageAccessRules, int pageId)
    {
        var filtered = pageAccessRules.Where(p => p.PageId == pageId);

        return filtered;
    }
}
