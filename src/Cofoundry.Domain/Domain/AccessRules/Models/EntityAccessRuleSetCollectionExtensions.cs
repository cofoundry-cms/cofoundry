namespace Cofoundry.Domain;

public static class EntityAccessRuleSetCollectionExtensions
{
    /// <summary>
    /// Filters the collection to include only rule sets not authorized to
    /// be accessed by the <paramref name="user"/>.
    /// </summary>
    /// <param name="accessRules">The <see cref="EntityAccessRule"/> collection to filer.</param>
    /// <param name="user">The <see cref="IUserContext"/> to filter on. Cannot be null.</param>
    /// <returns>A collection of rules that don't match the specified user account.</returns>
    public static IEnumerable<EntityAccessRuleSet> GetRuleViolations(this IEnumerable<EntityAccessRuleSet> accessRules, IUserContext user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return accessRules.Where(r => !r.IsAuthorized(user));
    }
}
