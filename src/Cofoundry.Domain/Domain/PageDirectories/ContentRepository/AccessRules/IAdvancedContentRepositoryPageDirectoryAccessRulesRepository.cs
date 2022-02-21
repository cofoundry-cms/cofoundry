using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for page access rules.
    /// </summary>
    public interface IAdvancedContentRepositoryPageDirectoryAccessRulesRepository
    {
        /// <summary>
        /// Updates all access rules associated with a page directory.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateAsync(UpdatePageDirectoryAccessRuleSetCommand command);

        /// <summary>
        /// Updates all access rules associated with a page directory.
        /// </summary>
        /// <param name="pageDirectoryId">
        /// Database id of the page directory to update.
        /// </param>
        /// <param name="commandPatcher">
        /// An action to configure or "patch" a command that's been initialized
        /// with the existing directory access rule data.
        /// </param>
        Task UpdateAsync(int pageDirectoryId, Action<UpdatePageDirectoryAccessRuleSetCommand> commandPatcher);
    }
}