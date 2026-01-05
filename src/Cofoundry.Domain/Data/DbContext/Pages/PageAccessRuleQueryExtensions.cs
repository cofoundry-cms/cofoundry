namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{PageAccessRule}"/>.
/// </summary>
public static class PageAccessRuleQueryExtensions
{
    extension(IQueryable<PageAccessRule> pageAccessRules)
    {
        /// <summary>
        /// Filters the collection to only include access rules associated with
        /// a specific page.
        /// </summary>
        /// <param name="pageId">Id of the <see cref="Page"/> to filter on.</param>
        public IQueryable<PageAccessRule> FilterByPageId(int pageId)
        {
            var filtered = pageAccessRules.Where(p => p.PageId == pageId);

            return filtered;
        }
    }
}
