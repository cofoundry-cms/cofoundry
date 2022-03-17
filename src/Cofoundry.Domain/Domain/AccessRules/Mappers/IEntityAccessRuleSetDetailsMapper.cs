using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Used for mapping the shared parts of an <see cref="IEntityAccessRuleSetDetails"/>
/// implementation for a specific entity.
/// </summary>
public interface IEntityAccessRuleSetDetailsMapper
{
    /// <summary>
    /// Maps the shared parts of an <see cref="IEntityAccessRuleSetDetails"/>
    /// implementation for a specific entity.
    /// </summary>
    /// <typeparam name="TAccessRule">
    /// The entity-specific <see cref="IEntityAccessRule"/> implementation 
    /// used by <paramref name="dbEntity"/> e.g. <see cref="PageAccessRule"/>.
    /// </typeparam>
    /// <typeparam name="TEntityAccessRuleSummary">
    /// The entity-specific <typeparamref name="TEntityAccessRuleSummary"/> 
    /// implementation to map to.
    /// </typeparam>
    /// <param name="dbEntity">The entity to map from.</param>
    /// <param name="result">
    /// The <see cref="IEntityAccessRuleSetDetails"/> projection to map to. Typically the mapper
    /// would create this instance, but passing it in means we don't need to specify all the
    /// generic parameters in the method call.
    /// </param>
    /// <param name="executionContext">The current query execution context to pass down to child queries.</param>
    /// <param name="ruleMapper">
    /// Mapping function that should finish off mapping any entity-specific properties
    /// in the rule projection e.g. for pages this is PageId and PageAccessRuleId.
    /// </param>
    Task MapAsync<TAccessRule, TEntityAccessRuleSummary>(
        IEntityAccessRestrictable<TAccessRule> dbEntity,
        IEntityAccessRuleSetDetails<TEntityAccessRuleSummary> result,
        IExecutionContext executionContext,
        Action<TAccessRule, TEntityAccessRuleSummary> ruleMapper
        )
        where TAccessRule : IEntityAccessRule
        where TEntityAccessRuleSummary : IEntityAccessRuleSummary, new();
}
