using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Helper for sharing functionality between the various commands 
    /// for updating access rules on entities that support this feature.
    /// </summary>
    public interface IUpdateAccessRulesCommandHelper
    {
        /// <summary>
        /// Updates the access rules and other related properties on the tracked 
        /// EF <paramref name="entity"/> using the values from the 
        /// <paramref name="command"/>. Changes are not saved to the database which
        /// needs to be done by the callee.
        /// </summary>
        /// <typeparam name="TAccessRule">
        /// The entity-specific type of the access rules being updated e.g. <see cref="PageAccessRule"/>.
        /// </typeparam>
        /// <typeparam name="TAddOrUpdateAccessRuleCommand">
        /// The entity-specific type of the access rule command data e.g. 
        /// <see cref="UpdatePageAccessRulesCommand.AddOrUpdateAccessRuleCommand"/>
        /// </typeparam>
        /// <param name="entity">The "tracked" EF entity to update properties on.</param>
        /// <param name="command">The command containing the updated data to apply to <paramref name="entity"/>.</param>
        /// <param name="executionContext">
        /// The current execution context of the request, which will be applied to any nested queries or commands.
        /// </param>
        Task UpdateAsync<TAccessRule, TAddOrUpdateAccessRuleCommand>(
            IEntityAccessRestrictable<TAccessRule> entity,
            UpdateAccessRulesCommandBase<TAddOrUpdateAccessRuleCommand> command,
            IExecutionContext executionContext
            )
            where TAccessRule : IEntityAccessRule, new()
            where TAddOrUpdateAccessRuleCommand : AddOrUpdateAccessRuleCommandBase;
    }
}
