using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.Data
{
    public static class IEntityAccessRuleQueryExtensions
    {
        /// <summary>
        /// Orders the access rule collection using the default ordering,
        /// which puts the most specific rules first i.e. role title ordered
        /// then by user area name.
        /// </summary>
        public static IOrderedQueryable<TEntity> OrderByDefault<TEntity>(this IQueryable<TEntity> accessRules)
            where TEntity : IEntityAccessRule
        {
            var filtered = accessRules
                .OrderBy(r => r.Role.Title)
                .ThenBy(r => r.UserArea.Name);

            return filtered;
        }

        /// <summary>
        /// Orders the access rule collection using the default ordering,
        /// which puts the most specific rules first i.e. role title ordered
        /// then by user area name.
        /// </summary>
        public static IOrderedEnumerable<TEntity> OrderByDefault<TEntity>(this IEnumerable<TEntity> accessRules)
            where TEntity : IEntityAccessRule
        {
            var filtered = accessRules
                .OrderBy(r => r.Role.Title)
                .ThenBy(r => r.UserArea.Name);

            return filtered;
        }
    }
}
