using System.Linq;

namespace Cofoundry.Domain.Data
{
    public static class PageDirectoryAccessRuleQueryExtensions
    {
        /// <summary>
        /// Filters the collection to only include the access rule with the 
        /// specified PageDirectoryAccessRuleId primary key.
        /// </summary>
        /// <param name="pageDirectoryAccessRuleId">Primary key to filter on.</param>
        public static IQueryable<PageDirectoryAccessRule> FilterById(this IQueryable<PageDirectoryAccessRule> accessRules, int pageDirectoryAccessRuleId)
        {
            var filtered = accessRules.Where(p => p.PageDirectoryAccessRuleId == pageDirectoryAccessRuleId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include access rules associated with
        /// a specific directory.
        /// </summary>
        /// <param name="pageDirectoryId">Id of the <see cref="PageDirectory"/> to filter on.</param>
        public static IQueryable<PageDirectoryAccessRule> FilterByPageId(this IQueryable<PageDirectoryAccessRule> accessRules, int pageDirectoryId)
        {
            var filtered = accessRules.Where(p => p.PageDirectoryId == pageDirectoryId);

            return filtered;
        }
    }
}
