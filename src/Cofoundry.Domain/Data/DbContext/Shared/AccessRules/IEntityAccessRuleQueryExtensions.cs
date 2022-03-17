namespace Cofoundry.Domain.Data;

public static class IEntityAccessRuleQueryExtensions
{
    /// <summary>
    /// Filters the collection to only include the access rule with the 
    /// specified PageAccessRuleId primary key.
    /// </summary>
    /// <param name="id">Primary key to filter on.</param>
    public static IQueryable<TEntity> FilterById<TEntity>(this IQueryable<TEntity> rules, int id)
        where TEntity : IEntityAccessRule
    {
        var filtered = rules.Where(p => p.GetId() == id);

        return filtered;
    }

    /// <summary>
    /// Filters the collection to only include the access rule with the 
    /// specified PageAccessRuleId primary key.
    /// </summary>
    /// <param name="id">Primary key to filter on.</param>
    public static IEnumerable<TEntity> FilterById<TEntity>(this IEnumerable<TEntity> rules, int id)
        where TEntity : IEntityAccessRule
    {
        var filtered = rules.Where(p => p.GetId() == id);

        return filtered;
    }

    /// <summary>
    /// Orders the access rule collection using the default ordering
    /// i.e. by user area, then by role.
    /// </summary>
    public static IOrderedQueryable<TEntity> OrderByDefault<TEntity>(this IQueryable<TEntity> accessRules)
        where TEntity : IEntityAccessRule
    {
        var filtered = accessRules
            .OrderBy(r => r.UserAreaCode)
            .ThenBy(r => r.RoleId);

        return filtered;
    }

    /// <summary>
    /// Orders the access rule collection using the default ordering
    /// i.e. by user area, then by role.
    /// </summary>
    public static IOrderedEnumerable<TEntity> OrderByDefault<TEntity>(this IEnumerable<TEntity> accessRules)
        where TEntity : IEntityAccessRule
    {
        var filtered = accessRules
            .OrderBy(r => r.UserAreaCode)
            .ThenBy(r => r.RoleId);

        return filtered;
    }
}
