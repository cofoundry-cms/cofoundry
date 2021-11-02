using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Common mapping functionality for <see cref="EntityAccessRule"/> projections.
    /// </summary>
    public interface IEntityAccessRuleSetMapper
    {
        /// <summary>
        /// Maps an entity with access restrictions to a new <see cref="EntityAccessRuleSet"/> 
        /// instance. If there are no access rules, then null is returned. The AccessRules collection 
        /// must be included in the <paramref name="entity"/> EF query.
        /// </summary>
        /// <param name="entity">
        /// The entity to map from. Cannot be null. The AccessRules collection must be included in the 
        /// <paramref name="entity"/> EF query.
        /// </param>
        EntityAccessRuleSet Map<TAccessRule>(IEntityAccessRestrictable<TAccessRule> entity)
            where TAccessRule : IEntityAccessRule;
    }
}
