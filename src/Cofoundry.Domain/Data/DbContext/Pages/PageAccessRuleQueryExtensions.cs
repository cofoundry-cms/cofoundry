using System.Linq;

namespace Cofoundry.Domain.Data
{
    public static class PageAccessRuleQueryExtensions
    {
        /// <summary>
        /// Filters the collection to only include the access rule with the 
        /// specified PageAccessRuleId primary key.
        /// </summary>
        /// <param name="pageAccessRuleId">Primary key to filter on.</param>
        public static IQueryable<PageAccessRule> FilterById(this IQueryable<PageAccessRule> pageAccessRules, int pageAccessRuleId)
        {
            var filtered = pageAccessRules.Where(p => p.PageAccessRuleId == pageAccessRuleId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include access rules associated with
        /// a specific page.
        /// </summary>
        /// <param name="pageId">Id of the <see cref="Page"/> to filter on.</param>
        public static IQueryable<PageAccessRule> FilterByPageId(this IQueryable<PageAccessRule> pageAccessRules, int pageId)
        {
            var filtered = pageAccessRules.Where(p => p.PageId == pageId);

            return filtered;
        }
    }
}
