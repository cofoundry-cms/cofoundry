namespace Cofoundry.Domain.Data;

/// <summary>
/// Extension methods for querying entities that implement <see cref="IEntityAccessRule"/>.
/// </summary>
public static class IEntityAccessRuleQueryExtensions
{
    extension<TEntity>(IQueryable<TEntity> rules) where TEntity : IEntityAccessRule
    {
        /// <summary>
        /// Filters the collection to only include the access rule with the 
        /// specified PageAccessRuleId primary key.
        /// </summary>
        /// <param name="id">Primary key to filter on.</param>
        public IQueryable<TEntity> FilterById(int id)
        {
            var filtered = rules.Where(p => p.GetId() == id);

            return filtered;
        }
    }

    extension<TEntity>(IEnumerable<TEntity> rules) where TEntity : IEntityAccessRule
    {
        /// <summary>
        /// Filters the collection to only include the access rule with the 
        /// specified PageAccessRuleId primary key.
        /// </summary>
        /// <param name="id">Primary key to filter on.</param>
        public IEnumerable<TEntity> FilterById(int id)
        {
            var filtered = rules.Where(p => p.GetId() == id);

            return filtered;
        }
    }

    extension<TEntity>(IQueryable<TEntity> accessRules) where TEntity : IEntityAccessRule
    {
        /// <summary>
        /// Orders the access rule collection using the default ordering
        /// i.e. by user area, then by role.
        /// </summary>
        public IOrderedQueryable<TEntity> OrderByDefault()
        {
            var filtered = accessRules
                .OrderBy(r => r.UserAreaCode)
                .ThenBy(r => r.RoleId);

            return filtered;
        }
    }

    extension<TEntity>(IEnumerable<TEntity> accessRules) where TEntity : IEntityAccessRule
    {
        /// <summary>
        /// Orders the access rule collection using the default ordering
        /// i.e. by user area, then by role.
        /// </summary>
        public IOrderedEnumerable<TEntity> OrderByDefault()
        {
            var filtered = accessRules
                .OrderBy(r => r.UserAreaCode)
                .ThenBy(r => r.RoleId);

            return filtered;
        }
    }
}
