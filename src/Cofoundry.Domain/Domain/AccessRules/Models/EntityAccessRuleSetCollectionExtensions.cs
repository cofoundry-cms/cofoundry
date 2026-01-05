namespace Cofoundry.Domain;

/// <summary>
/// Extension methods for collections of <see cref="EntityAccessRuleSet"/>.
/// </summary>
public static class EntityAccessRuleSetCollectionExtensions
{
    extension(IEnumerable<EntityAccessRuleSet> accessRules)
    {
        /// <summary>
        /// Filters the collection to include only rule sets not authorized to
        /// be accessed by the <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The <see cref="IUserContext"/> to filter on. Cannot be null.</param>
        /// <returns>A collection of rules that don't match the specified user account.</returns>
        public IEnumerable<EntityAccessRuleSet> GetRuleViolations(IUserContext user)
        {
            ArgumentNullException.ThrowIfNull(user);

            return accessRules.Where(r => !r.IsAuthorized(user));
        }
    }
}
