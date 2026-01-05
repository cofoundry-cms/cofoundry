namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for <see cref="IQueryable{PageDirectoryAccessRule}"/>.
/// </summary>
public static class PageDirectoryAccessRuleQueryExtensions
{
    extension(IQueryable<PageDirectoryAccessRule> accessRules)
    {
        /// <summary>
        /// Filters the collection to only include the access rule with the 
        /// specified PageDirectoryAccessRuleId primary key.
        /// </summary>
        /// <param name="pageDirectoryAccessRuleId">Primary key to filter on.</param>
        public IQueryable<PageDirectoryAccessRule> FilterById(int pageDirectoryAccessRuleId)
        {
            var filtered = accessRules.Where(p => p.PageDirectoryAccessRuleId == pageDirectoryAccessRuleId);

            return filtered;
        }

        /// <summary>
        /// Filters the collection to only include access rules associated with
        /// a specific directory.
        /// </summary>
        /// <param name="pageDirectoryId">Id of the <see cref="PageDirectory"/> to filter on.</param>
        public IQueryable<PageDirectoryAccessRule> FilterByPageDirectoryId(int pageDirectoryId)
        {
            var filtered = accessRules.Where(p => p.PageDirectoryId == pageDirectoryId);

            return filtered;
        }
    }
}
